using Company.Shared.Clients.ProductService.Protos;

namespace Company.Shared.Clients.ProductService;

public interface IProductServiceClient
{
    Task<ProductView> GetProductAsync(int id);
    Task<ProductView> CreateProductAsync(string name, double price, string description);
    Task<ProductView> UpdateProductAsync(int id, string name, double price, string description);
    Task DeleteProductAsync(int id);
    Task<ProductView[]> ListProductsAsync();
}