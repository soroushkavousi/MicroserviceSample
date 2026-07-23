using Company.ProductService.Extensions;
using Company.ProductService.Services;
using Company.Shared.ValueObjects;

namespace Company.ProductService.Endpoints;

public static class DeleteProductEndpoint
{
    public static void MapDeleteProduct(this RouteGroupBuilder group)
    {
        group.MapDelete("/{id:long}", HandleAsync)
            .WithName("DeleteProduct")
            .WithSummary("Delete product")
            .WithDescription("Deletes a product from the catalog by id.")
            .Produces<SuccessResultDto>()
            .Produces<Result>(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAsync(IProductService productService, long id,
        CancellationToken cancellationToken = default)
    {
        Result result = await productService.DeleteProductAsync(id, cancellationToken);
        return result.ToHttpResponse();
    }
}