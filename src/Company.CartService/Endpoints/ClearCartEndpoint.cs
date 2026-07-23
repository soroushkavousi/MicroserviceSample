using Company.CartService.Extensions;
using Company.CartService.Models.Dtos;
using Company.CartService.Services;
using Company.Shared.Extensions;
using Company.Shared.ValueObjects;

namespace Company.CartService.Endpoints;

public static class ClearCartEndpoint
{
    public static void MapClearCart(this RouteGroupBuilder group)
    {
        group.MapDelete("/", HandleAsync)
            .WithName("ClearCart")
            .WithSummary("Clear cart")
            .WithDescription("Removes all items from the current user's cart.")
            .Produces<SuccessResultDto<CartDto>>()
            .Produces<Result>(StatusCodes.Status401Unauthorized);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext, ICartService cartService, CancellationToken cancellationToken)
    {
        if (!httpContext.TryGetUserId(out long userId))
            return Results.Unauthorized();

        Result<CartDto> result = await cartService.ClearCartAsync(userId, cancellationToken);
        return result.ToHttpResponse();
    }
}