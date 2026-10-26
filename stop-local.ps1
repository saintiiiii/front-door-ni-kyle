$stateFile = Join-Path $env:TEMP 'fixit-local-processes.json'
if (-not (Test-Path -LiteralPath $stateFile)) {
    Write-Output 'No FIXIT local process list was found.'
    exit 0
}
$state = Get-Content -LiteralPath $stateFile -Raw | ConvertFrom-Json
$apiChildren = Get-CimInstance Win32_Process -ErrorAction SilentlyContinue | Where-Object { $_.ParentProcessId -eq $state.api -and $_.Name -eq 'FIXIT.API.exe' }
foreach ($child in $apiChildren) { Stop-Process -Id $child.ProcessId -Force -ErrorAction SilentlyContinue }
foreach ($id in @($state.web, $state.api)) {
    if ($null -eq $id) { continue }
    $process = Get-CimInstance Win32_Process -Filter "ProcessId = $id" -ErrorAction SilentlyContinue
    if ($null -eq $process) { continue }
    $isFixitApi = $process.CommandLine -match 'FIXIT\.API'
    $isFixitWeb = $process.CommandLine -match 'node_modules[\\/]vite[\\/]bin[\\/]vite\.js'
    if ($isFixitApi -or $isFixitWeb) { Stop-Process -Id $id -Force -ErrorAction SilentlyContinue }
}
Remove-Item -LiteralPath $stateFile -Force -ErrorAction SilentlyContinue
Write-Output 'FIXIT local services stopped.'
