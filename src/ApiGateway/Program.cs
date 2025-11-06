using ApiGateway;
using Company.Shared.Clients.ProductService;
using Company.Shared.Clients.ProductService.Models;
using Company.Shared.Clients.ProductService.Protos;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Register endpoint explorer and Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register ProductService gRPC client
builder.Services.AddSingleton<IProductServiceClient>(sp =>
    new ProductServiceClient("https://localhost:7251")); // replace with your ProductService address

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- Product CRUD endpoints ---

app.MapGet("/products", async (IProductServiceClient client, string phrase,
        int page = 1, int pageSize = 10) =>
    {
        Result<ProductView[]> result = await client.ListProductsAsync
        (
            phrase: phrase,
            page: page,
            pageSize: pageSize
        );
        return result.HasError ? result.ToApiErrorResponse() : Results.Ok(result);
    })
    .WithName("GetProducts")
    .WithOpenApi(); // Adds OpenAPI metadata for Swagger docs

app.MapGet("/products/{id:int}", async (IProductServiceClient client, int id) =>
    {
        Result<ProductView> result = await client.GetProductAsync(id);
        return result.HasError ? result.ToApiErrorResponse() : Results.Ok(result);
    })
    .WithName("GetProduct")
    .WithOpenApi();

app.MapPost("/products", async (IProductServiceClient client, CreateProductDto dto) =>
    {
        Result<ProductView> result = await client.CreateProductAsync(dto.Name, dto.Price, dto.Description);
        return result.HasError
            ? result.ToApiErrorResponse()
            : Results.Created($"/api/products/{result.Data.Id}", result);
    })
    .WithName("CreateProduct")
    .WithOpenApi();

app.MapPut("/products/{id:int}", async (IProductServiceClient client, int id, UpdateProductDto dto) =>
    {
        Result<ProductView> result = await client.UpdateProductAsync(id, dto.Name, dto.Price, dto.Description);
        return result.HasError ? result.ToApiErrorResponse() : Results.Ok(result);
    })
    .WithName("UpdateProduct")
    .WithOpenApi();

app.MapDelete("/products/{id:int}", async (IProductServiceClient client, int id) =>
    {
        Result result = await client.DeleteProductAsync(id);
        return result.HasError ? result.ToApiErrorResponse() : Results.Ok(result);
    })
    .WithName("DeleteProduct")
    .WithOpenApi();

app.Run();

// --- DTOs for request bodies ---
internal record CreateProductDto(string Name, double Price, string Description);

internal record UpdateProductDto(string Name, double Price, string Description);