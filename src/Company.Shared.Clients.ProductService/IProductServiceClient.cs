using Company.Shared.Clients.ProductService.Models;
using Company.Shared.Clients.ProductService.Protos;

namespace Company.Shared.Clients.ProductService;

public interface IProductServiceClient
{
    Task<Result<ProductView>> GetProductAsync(int id);
    Task<Result<ProductView>> CreateProductAsync(string name, double price, string description);
    Task<Result<ProductView>> UpdateProductAsync(int id, string name, double price, string description);
    Task<Result> DeleteProductAsync(int id);
    Task<Result<ProductView[]>> ListProductsAsync(string phrase = null, int page = 1, int pageSize = 10);
}