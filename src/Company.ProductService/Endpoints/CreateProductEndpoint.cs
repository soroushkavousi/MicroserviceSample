using Company.ProductService.Extensions;
using Company.ProductService.Models.Dtos;
using Company.ProductService.Services;
using Company.Shared.ValueObjects;

namespace Company.ProductService.Endpoints;

public static class CreateProductEndpoint
{
    public static void MapCreateProduct(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .WithName("CreateProduct")
            .WithSummary("Create product")
            .WithDescription("Creates a new product in the catalog.")
            .Produces<SuccessResultDto<ProductDto>>()
            .Produces<Result>(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> HandleAsync(
        IProductService productService, CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        Result<ProductDto> result = await productService.CreateProductAsync(
            dto.Name, dto.Price, dto.Description, cancellationToken);
        return result.ToHttpResponse();
    }
}