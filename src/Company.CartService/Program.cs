using Company.CartService.Endpoints;
using Company.CartService.Extensions;
using Company.CartService.Services;
using Company.Shared.Extensions;
using Company.Shared.ProductService;
using DotNetPotion.AppEnvironmentPack;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureSharedHttpJsonOptions();

string productServiceAddress = builder.Configuration["ProductService:GrpcAddress"]
    ?? "https://localhost:7251";

builder.Services.AddSingleton<IProductServiceClient>(_ => new ProductServiceClient(productServiceAddress));
builder.Services.AddSingleton<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddCartServiceOpenApi();

WebApplication app = builder.Build();

if (!AppEnvironment.IsProduction)
    app.MapOpenApi();

app.MapCartEndpoints();

app.Run();