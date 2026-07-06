using Company.CartService.Extensions;
using Company.CartService.Models.Dtos;
using Company.CartService.Services;
using Company.Shared.Extensions;
using Company.Shared.ValueObjects;

namespace Company.CartService.Endpoints;

public static class AddCartItemEndpoint
{
    public static void MapAddCartItem(this RouteGroupBuilder group)
    {
        group.MapPost("/items", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext, ICartService cartService, AddCartItemDto dto,
        CancellationToken cancellationToken)
    {
        if (!httpContext.TryGetUserId(out long userId))
            return Results.Unauthorized();

        Result<CartDto> result = await cartService.AddItemAsync(userId, dto.ProductId, dto.Quantity,
            cancellationToken);
        return result.ToHttpResponse();
    }
}
