using Company.Shared.ProductService.Protos;
using Company.Shared.ValueObjects;

namespace Company.Shared.ProductService;

public interface IProductServiceClient
{
    Task<Result<ProductContractDto>> GetProductAsync(long id);

    Task<Result<ProductContractDto>> CreateProductAsync(string name, double price,
        string description);

    Task<Result<ProductContractDto>> UpdateProductAsync(long id, string name, double price,
        string description);

    Task<Result> DeleteProductAsync(long id);

    Task<Result<ProductContractDto[]>> ListProductsAsync(string phrase = null, int page = 1,
        int pageSize = 10);

    Task<Result<ProductContractDto[]>> ListProductsByIdsAsync(long[] ids);
}