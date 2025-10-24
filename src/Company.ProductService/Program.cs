using Company.ProductService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

WebApplication app = builder.Build();

app.MapGrpcService<ProductServiceGrpc>();
app.MapGet("/", () => "Company.ProductService (gRPC) is running.");

app.Run();