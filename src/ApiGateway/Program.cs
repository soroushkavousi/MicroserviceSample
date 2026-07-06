using ApiGateway.Configs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddConfigFilter<ClusterDefaultsConfigFilter>();

WebApplication app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    gateway = "Company.ApiGateway",
    routes = new object[]
    {
        new { path = "/products/**", destination = "Company.ProductService" },
        new { path = "/cart/**", destination = "Company.CartService" }
    }
}));

app.MapReverseProxy();

app.Run();