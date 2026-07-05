using Company.ProductService.Extensions;
using Company.ProductService.Models.Dtos;
using Company.ProductService.Services;
using Company.Shared.ValueObjects;

namespace Company.ProductService.Endpoints;

public static class ListProductsEndpoint
{
    public static void MapListProducts(this RouteGroupBuilder group)
    {
        group.MapGet("/", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        IProductService productService, string phrase, int page = 1, int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        Result<ProductDto[]> listResult = await productService.ListProductsAsync(
            phrase, page, pageSize, cancellationToken);
        return listResult.ToHttpResponse();
    }
}