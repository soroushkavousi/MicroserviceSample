using Company.ProductService.Extensions;
using Company.ProductService.Models.Dtos;
using Company.ProductService.Services;
using Company.Shared.ValueObjects;

namespace Company.ProductService.Endpoints;

public static class UpdateProductEndpoint
{
    public static void MapUpdateProduct(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:long}", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(IProductService productService, long id,
        UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        Result<ProductDto> result = await productService.UpdateProductAsync(
            id, dto.Name, dto.Price, dto.Description, cancellationToken);
        return result.ToHttpResponse();
    }
}