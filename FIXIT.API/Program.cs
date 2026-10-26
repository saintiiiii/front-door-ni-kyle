using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;
using FIXIT.Domain.Entities;
using FIXIT.Infrastructure;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Antiforgery;
using System.Security.Claims;
using FIXIT.API;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAntiforgery(o => o.HeaderName = "X-XSRF-TOKEN");
builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var databaseProvider = builder.Configuration["Database:Provider"] ?? "SqlServer";
var databaseConnection = builder.Configuration.GetConnectionString("Fixit") ?? "Server=(localdb)\\mssqllocaldb;Database=FIXIT;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
builder.Services.AddDbContext<FIXITDbContext>(o =>
{
    if (databaseProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase)) o.UseMySQL(databaseConnection);
    else if (databaseProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase)) o.UseSqlServer(databaseConnection);
    else throw new InvalidOperationException($"Unsupported database provider '{databaseProvider}'. Use SqlServer or MySql.");
});
builder.Services.AddIdentity<User, IdentityRole>(o => { o.Password.RequiredLength=12; o.Password.RequireDigit=true; o.Password.RequireUppercase=true; o.Lockout.MaxFailedAccessAttempts=5; o.User.RequireUniqueEmail=true; })
 .AddEntityFrameworkStores<FIXITDbContext>().AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(o => { o.Cookie.HttpOnly=true; o.Cookie.SecurePolicy=builder.Environment.IsDevelopment()?CookieSecurePolicy.SameAsRequest:CookieSecurePolicy.Always; o.Cookie.SameSite=SameSiteMode.Strict; o.Events.OnValidatePrincipal=async context=>{var manager=context.HttpContext.RequestServices.GetRequiredService<UserManager<User>>();var user=await manager.GetUserAsync(context.Principal!);if(user is null||!user.IsActive){context.RejectPrincipal();await context.HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);}};o.Events.OnRedirectToLogin=context=>{if(context.Request.Path.StartsWithSegments("/api")){context.Response.StatusCode=StatusCodes.Status401Unauthorized;return Task.CompletedTask;}context.Response.Redirect(context.RedirectUri);return Task.CompletedTask;};o.Events.OnRedirectToAccessDenied=context=>{if(context.Request.Path.StartsWithSegments("/api")){context.Response.StatusCode=StatusCodes.Status403Forbidden;return Task.CompletedTask;}context.Response.Redirect(context.RedirectUri);return Task.CompletedTask;}; });
builder.Services.AddAuthorization(o => { o.AddPolicy("AdministratorOnly", p=>p.RequireRole("Administrator")); o.AddPolicy("ApprovedTechnician", p=>p.RequireRole("Technician")); });
builder.Services.AddRateLimiter(o => o.AddFixedWindowLimiter("api", x=>{x.PermitLimit=100;x.Window=TimeSpan.FromMinutes(1);x.QueueLimit=0;}));
builder.Services.AddProblemDetails();
var app=builder.Build();
if (databaseProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase))
{
    var mysqlSettings = new MySqlConnectionStringBuilder(databaseConnection);
    var databaseName = mysqlSettings.Database;
    if (string.IsNullOrWhiteSpace(databaseName)) throw new InvalidOperationException("Set a database name in the MySQL connection string.");
    mysqlSettings.Database = string.Empty;
    await using (var connection = new MySqlConnection(mysqlSettings.ConnectionString))
    {
        await connection.OpenAsync();
        await using var createDatabase = connection.CreateCommand();
        createDatabase.CommandText = $"CREATE DATABASE IF NOT EXISTS `{databaseName.Replace("`", "``")}` CHARACTER SET utf8mb4;";
        await createDatabase.ExecuteNonQueryAsync();
    }
    using var scope = app.Services.CreateScope();
    await scope.ServiceProvider.GetRequiredService<FIXITDbContext>().Database.EnsureCreatedAsync();
}
app.UseExceptionHandler();
app.Use(async (ctx,next)=>{if((HttpMethods.IsPost(ctx.Request.Method)||HttpMethods.IsPut(ctx.Request.Method)||HttpMethods.IsPatch(ctx.Request.Method)||HttpMethods.IsDelete(ctx.Request.Method))&&ctx.Request.Path.StartsWithSegments("/api")){try{await ctx.RequestServices.GetRequiredService<IAntiforgery>().ValidateRequestAsync(ctx);}catch(AntiforgeryValidationException){ctx.Response.StatusCode=StatusCodes.Status400BadRequest;await ctx.Response.WriteAsJsonAsync(new{message="A valid CSRF token is required."});return;}}await next();});
app.Use(async (ctx,next)=>{ctx.Response.Headers["X-Content-Type-Options"]="nosniff";ctx.Response.Headers["X-Frame-Options"]="DENY";ctx.Response.Headers["Referrer-Policy"]="no-referrer";await next();});
if(app.Environment.IsDevelopment()){app.UseSwagger();app.UseSwaggerUI();}
app.UseHttpsRedirection(); app.UseRateLimiter(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers().RequireRateLimiting("api");
app.MapGet("/health",()=>Results.Ok(new {status="ok",system="FIXIT",timeZone="Asia/Manila"}));
var bootstrapEmail=builder.Configuration["FIXIT_BOOTSTRAP_ADMIN_EMAIL"];
var bootstrapPassword=builder.Configuration["FIXIT_BOOTSTRAP_ADMIN_PASSWORD"];
if(!string.IsNullOrWhiteSpace(bootstrapEmail)&&!string.IsNullOrWhiteSpace(bootstrapPassword))
{
    using var scope=app.Services.CreateScope();
    var db=scope.ServiceProvider.GetRequiredService<FIXITDbContext>();
    if (databaseProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase)) await db.Database.EnsureCreatedAsync();
    else await db.Database.MigrateAsync();
    var manager=scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    if(await manager.FindByEmailAsync(bootstrapEmail) is null)
    {
        var admin=new User{UserName=bootstrapEmail,Email=bootstrapEmail,EmailConfirmed=true,FirstName="FIXIT",LastName="Administrator"};
        var result=await manager.CreateAsync(admin,bootstrapPassword);
        if(!result.Succeeded)throw new InvalidOperationException("Unable to create configured FIXIT administrator. Check the configured password policy.");
        await manager.AddToRoleAsync(admin,"Administrator");
    }
}
var seedDemoData = builder.Configuration.GetValue<bool>("FIXIT_SEED_DEMO_DATA");
if (seedDemoData)
{
    if (!app.Environment.IsDevelopment())
        throw new InvalidOperationException("FIXIT_SEED_DEMO_DATA can only be enabled in the Development environment.");
    var demoPassword = builder.Configuration["FIXIT_DEMO_PASSWORD"];
    if (string.IsNullOrWhiteSpace(demoPassword))
        throw new InvalidOperationException("Set FIXIT_DEMO_PASSWORD when FIXIT_SEED_DEMO_DATA is enabled.");

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<FIXITDbContext>();
    if (databaseProvider.Equals("MySql", StringComparison.OrdinalIgnoreCase)) await db.Database.EnsureCreatedAsync();
    else await db.Database.MigrateAsync();
    await DemoDataSeeder.SeedAsync(db, scope.ServiceProvider.GetRequiredService<UserManager<User>>(), demoPassword);
}
app.Run();

public partial class Program { }
