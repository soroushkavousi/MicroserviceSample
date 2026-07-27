using ApiGateway.Configs;
using ApiGateway.Docs;
using ApiGateway.Identity;
using ApiGateway.Proxy;
using Company.Shared.Extensions;
using DotNetPotion.AppEnvironmentPack;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelemetry("api-gateway");

builder.Services
    .AddAuthentication(FakeJwtAuthenticationDefaults.AuthenticationScheme)
    .AddScheme<FakeJwtOptions, FakeJwtAuthenticationHandler>(
        FakeJwtAuthenticationDefaults.AuthenticationScheme, null);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        GatewayAuthorizationPolicies.AuthenticatedUser,
        policy => policy.RequireAuthenticatedUser());
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddConfigFilter<ClusterDefaultsConfigFilter>()
    .AddTransforms(builder => builder.AddUserIdentityHeader());

WebApplication app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if (!AppEnvironment.IsProduction)
    app.MapApiDocs();

app.MapMetrics();
app.MapReverseProxy();

app.Run();