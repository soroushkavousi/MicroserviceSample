using Company.Shared.Clients.ProductService;
using Company.Shared.Clients.ProductService.Protos;
using Grpc.Core;

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

app.MapGet("/products", async (IProductServiceClient client) =>
    {
        ProductView[] products = await client.ListProductsAsync();
        return Results.Ok(products);
    })
    .WithName("GetProducts")
    .WithOpenApi(); // Adds OpenAPI metadata for Swagger docs

app.MapGet("/products/{id:int}", async (int id, IProductServiceClient client) =>
    {
        try
        {
            ProductView product = await client.GetProductAsync(id);
            return Results.Ok(product);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return Results.NotFound($"Product {id} not found");
        }
    })
    .WithName("GetProduct")
    .WithOpenApi();

app.MapPost("/products", async (CreateProductDto dto, IProductServiceClient client) =>
    {
        ProductView product = await client.CreateProductAsync(dto.Name, dto.Price, dto.Description);
        return Results.Created($"/api/products/{product.Id}", product);
    })
    .WithName("CreateProduct")
    .WithOpenApi();

app.MapPut("/products/{id:int}", async (int id, UpdateProductDto dto, IProductServiceClient client) =>
    {
        try
        {
            ProductView product = await client.UpdateProductAsync(id, dto.Name, dto.Price, dto.Description);
            return Results.Ok(product);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return Results.NotFound($"Product {id} not found");
        }
    })
    .WithName("UpdateProduct")
    .WithOpenApi();

app.MapDelete("/products/{id:int}", async (int id, IProductServiceClient client) =>
    {
        try
        {
            await client.DeleteProductAsync(id);
            return Results.NoContent();
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return Results.NotFound($"Product {id} not found");
        }
    })
    .WithName("DeleteProduct")
    .WithOpenApi();

// --- Run the app ---
app.Run();

// --- DTOs for request bodies ---
internal record CreateProductDto(string Name, double Price, string Description);

internal record UpdateProductDto(string Name, double Price, string Description);