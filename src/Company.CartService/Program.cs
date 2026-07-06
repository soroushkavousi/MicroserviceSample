using Company.CartService.Endpoints;
using Company.CartService.Services;
using Company.Shared.Extensions;
using Company.Shared.ProductService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureSharedHttpJsonOptions();

string productServiceAddress = builder.Configuration["ProductService:GrpcAddress"]
    ?? "https://localhost:7251";

builder.Services.AddSingleton<IProductServiceClient>(_ => new ProductServiceClient(productServiceAddress));
builder.Services.AddSingleton<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();

WebApplication app = builder.Build();

app.MapCartEndpoints();
app.MapGet("/", () => Results.Ok(new { service = "Company.CartService" }));

app.Run();