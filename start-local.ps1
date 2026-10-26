$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$frontend = Join-Path $root 'frontend'
$node = 'C:\Program Files\nodejs\node.exe'
$vite = Join-Path $frontend 'node_modules\vite\bin\vite.js'
$stateFile = Join-Path $env:TEMP 'fixit-local-processes.json'
$apiLog = Join-Path $env:TEMP 'fixit-api.log'
$apiErrorLog = Join-Path $env:TEMP 'fixit-api-error.log'
$webLog = Join-Path $env:TEMP 'fixit-frontend.log'
$webErrorLog = Join-Path $env:TEMP 'fixit-frontend-error.log'

if (-not (Test-Path -LiteralPath $node) -or -not (Test-Path -LiteralPath $vite)) {
    throw 'Node.js or frontend dependencies are missing. Install Node.js, then run npm ci in the frontend folder.'
}
if ([string]::IsNullOrWhiteSpace($env:MYSQL_HOST)) { $env:MYSQL_HOST = '127.0.0.1' }
if ([string]::IsNullOrWhiteSpace($env:MYSQL_PORT)) { $env:MYSQL_PORT = '3306' }
if ([string]::IsNullOrWhiteSpace($env:MYSQL_USER)) { $env:MYSQL_USER = Read-Host 'MySQL username' }
if ([string]::IsNullOrWhiteSpace($env:MYSQL_PASSWORD)) {
    $securePassword = Read-Host 'MySQL password' -AsSecureString
    $env:MYSQL_PASSWORD = [System.Net.NetworkCredential]::new('', $securePassword).Password
}

Push-Location $root
try {
    dotnet build FIXIT.slnx
    if ($LASTEXITCODE -ne 0) { throw 'The FIXIT solution did not build.' }
} finally {
    Pop-Location
}

$env:Database__Provider = 'MySql'
$env:ConnectionStrings__Fixit = "Server=$env:MYSQL_HOST;Port=$env:MYSQL_PORT;Database=fixit_local;User ID=$env:MYSQL_USER;Password=$env:MYSQL_PASSWORD"
$env:FIXIT_SEED_DEMO_DATA = 'true'
$env:FIXIT_DEMO_PASSWORD = 'Fixit-Demo-2026!'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:ASPNETCORE_URLS = 'http://localhost:5148'
$api = Start-Process -FilePath 'dotnet' -ArgumentList @('run', '--project', 'FIXIT.API', '--no-build', '--no-launch-profile') -WorkingDirectory $root -WindowStyle Hidden -PassThru -RedirectStandardOutput $apiLog -RedirectStandardError $apiErrorLog
$web = Start-Process -FilePath $node -ArgumentList @('node_modules/vite/bin/vite.js', '--host', '127.0.0.1') -WorkingDirectory $frontend -WindowStyle Hidden -PassThru -RedirectStandardOutput $webLog -RedirectStandardError $webErrorLog
Set-Content -LiteralPath $stateFile -Value (@{ api = $api.Id; web = $web.Id } | ConvertTo-Json) -Encoding utf8

$ready = $false
for ($i = 0; $i -lt 30; $i++) {
    Start-Sleep -Seconds 1
    try {
        $health = Invoke-RestMethod -Uri 'http://localhost:5148/health' -TimeoutSec 2
        $page = Invoke-WebRequest -Uri 'http://127.0.0.1:5173/' -UseBasicParsing -TimeoutSec 2
        if ($health.status -eq 'ok' -and $page.StatusCode -eq 200) { $ready = $true; break }
    } catch { }
    if ($api.HasExited -or $web.HasExited) { break }
}
if (-not $ready) {
    Get-Content -LiteralPath $apiLog -Tail 35 -ErrorAction SilentlyContinue
    Get-Content -LiteralPath $apiErrorLog -Tail 35 -ErrorAction SilentlyContinue
    Get-Content -LiteralPath $webLog -Tail 15 -ErrorAction SilentlyContinue
    Get-Content -LiteralPath $webErrorLog -Tail 15 -ErrorAction SilentlyContinue
    throw 'FIXIT did not start. Review the startup logs shown above.'
}

Write-Output 'FIXIT is running: http://localhost:5173'
Write-Output 'API health: http://localhost:5148/health'
Write-Output 'Customer and technician demo password: Fixit-Demo-2026!'
Write-Output "Stop both services with: .\stop-local.ps1"
