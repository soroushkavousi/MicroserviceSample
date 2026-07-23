using Company.CartService.Extensions;
using Company.CartService.Models.Dtos;
using Company.CartService.Services;
using Company.Shared.Extensions;
using Company.Shared.ValueObjects;

namespace Company.CartService.Endpoints;

public static class RemoveCartItemEndpoint
{
    public static void MapRemoveCartItem(this RouteGroupBuilder group)
    {
        group.MapDelete("/items/{productId:long}", HandleAsync)
            .WithName("RemoveCartItem")
            .WithSummary("Remove cart item")
            .WithDescription("Removes a product line from the current user's cart.")
            .Produces<SuccessResultDto<CartDto>>()
            .Produces<Result>(StatusCodes.Status404NotFound)
            .Produces<Result>(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext, ICartService cartService, long productId,
        CancellationToken cancellationToken)
    {
        if (!httpContext.TryGetUserId(out long userId))
            return Results.Unauthorized();

        Result<CartDto> result = await cartService.RemoveItemAsync(
            userId, productId, cancellationToken);
        return result.ToHttpResponse();
    }
}