using Company.ProductService.Extensions;
using Company.ProductService.Models.Dtos;
using Company.ProductService.Services;
using Company.Shared.ValueObjects;

namespace Company.ProductService.Endpoints;

public static class GetProductEndpoint
{
    public static void MapGetProduct(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:long}", HandleAsync)
            .WithName("GetProduct")
            .WithSummary("Get product")
            .WithDescription("Returns a single product by id.")
            .Produces<SuccessResultDto<ProductDto>>()
            .Produces<Result>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(IProductService productService, long id,
        CancellationToken cancellationToken = default)
    {
        Result<ProductDto> result = await productService.GetProductAsync(id, cancellationToken);
        return result.ToHttpResponse();
    }
}