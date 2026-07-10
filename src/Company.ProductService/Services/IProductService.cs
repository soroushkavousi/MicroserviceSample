using Company.ProductService.Models.Dtos;
using Company.Shared.ValueObjects;

namespace Company.ProductService.Services;

public interface IProductService
{
    Task<Result<ProductDto[]>> ListProductsAsync(string phrase, int page, int pageSize,
        CancellationToken cancellationToken);

    Task<Result<ProductDto[]>> ListProductsByIdsAsync(long[] ids,
        CancellationToken cancellationToken);

    Task<Result<ProductDto>> GetProductAsync(long id, CancellationToken cancellationToken);

    Task<Result<ProductDto>> CreateProductAsync(string name, double price, string description,
        CancellationToken cancellationToken);

    Task<Result<ProductDto>> UpdateProductAsync(long id, string name, double price,
        string description, CancellationToken cancellationToken);

    Task<Result> DeleteProductAsync(long id, CancellationToken cancellationToken);
}