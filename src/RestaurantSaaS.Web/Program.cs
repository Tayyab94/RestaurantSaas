using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RestaurantSaaS.Application;
using RestaurantSaaS.Application.Abstractions;
using RestaurantSaaS.Domain.Enums;
using RestaurantSaaS.Infrastructure;
using RestaurantSaaS.Infrastructure.Persistence;
using RestaurantSaaS.Shared.UI;
using RestaurantSaaS.Shared.UI.Services.Auth;
using RestaurantSaaS.Web.Components;
using RestaurantSaaS.Web.Endpoints;
using RestaurantSaaS.Web.Hubs;
using RestaurantSaaS.Web.Infrastructure;
using RestaurantSaaS.Web.Security;
using Serilog;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Fail fast when security settings are missing.
    var jwt = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
    jwt.Validate();
    builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection(JwtOptions.SectionName));

    // Application and data
    builder.Services
        .AddApplication()
        .AddLocalClientServices()
        .AddInfrastructure(builder.Configuration);

    // Who is calling: JWT claims for the API, the UI session for in-process calls
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<WebRequestContext>();
    builder.Services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<WebRequestContext>());
    builder.Services.AddScoped<ITenantProvider>(sp => sp.GetRequiredService<WebRequestContext>());
    builder.Services.AddSingleton<ITokenService, JwtTokenService>();
    builder.Services.AddScoped<IRequestInfo, WebRequestInfo>();

    // Cloud (default) or OnPremise (the server runs in the restaurant). Set with Deployment:Mode.
    var deploymentMode = Enum.TryParse<DeploymentMode>(builder.Configuration["Deployment:Mode"], true, out var configuredMode) && configuredMode != DeploymentMode.Desktop
        ? configuredMode
        : DeploymentMode.Cloud;
    builder.Services.AddSingleton<IDeploymentInfo>(new DeploymentInfo(deploymentMode));

    // Authentication for the API and SignalR
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SigningKey)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
                NameClaimType = "name",
                RoleClaimType = "role",
            };
        });
    builder.Services.AddAuthorization();

    // Web UI. The UI services are added after Razor components so the app's authentication state provider wins.
    builder.Services.AddRazorComponents().AddInteractiveServerComponents();
    builder.Services.AddSharedUi();
    builder.Services.AddScoped<ISecureStore, ProtectedLocalStorageSecureStore>();

    builder.Services.AddHostedService<DatabaseInitializerService>();
    builder.Services.AddSignalR();
    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddOpenApi();
    builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>("database");

    var app = builder.Build();

    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi(); // /openapi/v1.json
    }
    else
    {
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseAntiforgery();

    app.MapHealthChecks("/health");
    app.MapApiEndpoints();
    app.MapHub<PosHub>("/hubs/pos");

    app.MapRazorComponents<App>()
        .AddInteractiveServerRenderMode()
        .AddAdditionalAssemblies(typeof(Marker).Assembly);

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "The host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Makes the entry point visible to the integration tests (WebApplicationFactory<Program>).
public partial class Program
{
}
