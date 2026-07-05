using Company.CartService.Extensions;
using Company.CartService.Models.Dtos;
using Company.CartService.Services;
using Company.Shared.ValueObjects;

namespace Company.CartService.Endpoints;

public static class ClearCartEndpoint
{
    public static void MapClearCart(this RouteGroupBuilder group)
    {
        group.MapDelete("/{userId:long}", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(ICartService cartService, long userId,
        CancellationToken cancellationToken)
    {
        Result<CartDto> result = await cartService.ClearCartAsync(userId, cancellationToken);
        return result.ToHttpResponse();
    }
}