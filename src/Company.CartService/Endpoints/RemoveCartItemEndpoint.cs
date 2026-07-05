using Company.CartService.Extensions;
using Company.CartService.Models.Dtos;
using Company.CartService.Services;
using Company.Shared.ValueObjects;

namespace Company.CartService.Endpoints;

public static class RemoveCartItemEndpoint
{
    public static void MapRemoveCartItem(this RouteGroupBuilder group)
    {
        group.MapDelete("/{userId:long}/items/{productId:long}", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(
        ICartService cartService, long userId, long productId,
        CancellationToken cancellationToken)
    {
        Result<CartDto> result = await cartService.RemoveItemAsync(
            userId, productId, cancellationToken);
        return result.ToHttpResponse();
    }
}