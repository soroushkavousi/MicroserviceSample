using ApiGateway.Configs;
using ApiGateway.Identity;
using ApiGateway.Proxy;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

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

app.MapGet("/", () => Results.Ok(new
{
    gateway = "Company.ApiGateway",
    routes = new object[]
    {
        new { path = "/products/**", destination = "Company.ProductService", auth = "none" },
        new { path = "/cart/**", destination = "Company.CartService", auth = "Bearer {userId}" }
    }
}));

app.MapReverseProxy();

app.Run();
