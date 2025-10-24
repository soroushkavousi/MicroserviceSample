using Company.Shared.Clients.ProductService.Protos;
using Grpc.Net.Client;

namespace Company.Shared.Clients.ProductService;

public class ProductServiceClient : IProductServiceClient, IDisposable
{
    private readonly Protos.ProductService.ProductServiceClient _grpcClient;
    private readonly GrpcChannel _channel;

    public ProductServiceClient(string address)
    {
        _channel = GrpcChannel.ForAddress(address);
        _grpcClient = new(_channel);
    }

    public async Task<ProductView> GetProductAsync(int id)
    {
        GetProductRequest request = new() { Id = id };
        ProductView result = await _grpcClient.GetProductAsync(request);
        return result;
    }

    public async Task<ProductView> CreateProductAsync(string name, double price, string description)
    {
        CreateProductRequest request = new()
        {
            Name = name,
            Price = price,
            Description = description
        };
        return await _grpcClient.CreateProductAsync(request);
    }

    public async Task<ProductView> UpdateProductAsync(int id, string name, double price, string description)
    {
        UpdateProductRequest request = new()
        {
            Id = id,
            Name = name,
            Price = price,
            Description = description
        };
        return await _grpcClient.UpdateProductAsync(request);
    }

    public async Task DeleteProductAsync(int id)
    {
        DeleteProductRequest request = new() { Id = id };
        await _grpcClient.DeleteProductAsync(request);
    }

    public async Task<ProductView[]> ListProductsAsync()
    {
        ProductListView productViews = await _grpcClient.ListProductsAsync(new());
        return productViews.Items.ToArray();
    }

    public void Dispose() => _channel.Dispose();
}