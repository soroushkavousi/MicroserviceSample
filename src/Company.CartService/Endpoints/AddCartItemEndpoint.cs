using Company.CartService.Extensions;
using Company.CartService.Models.Dtos;
using Company.CartService.Services;
using Company.Shared.ValueObjects;

namespace Company.CartService.Endpoints;

public static class AddCartItemEndpoint
{
    public static void MapAddCartItem(this RouteGroupBuilder group)
    {
        group.MapPost("/{userId:long}/items", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(ICartService cartService, long userId,
        AddCartItemDto dto, CancellationToken cancellationToken)
    {
        Result<CartDto> result = await cartService.AddItemAsync(userId, dto.ProductId, dto.Quantity,
            cancellationToken);
        return result.ToHttpResponse();
    }
}