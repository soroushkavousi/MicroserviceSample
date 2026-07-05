using Company.CartService.Extensions;
using Company.CartService.Models.Dtos;
using Company.CartService.Services;
using Company.Shared.ValueObjects;

namespace Company.CartService.Endpoints;

public static class GetCartEndpoint
{
    public static void MapGetCart(this RouteGroupBuilder group)
    {
        group.MapGet("/{userId:long}", HandleAsync);
    }

    private static async Task<IResult> HandleAsync(ICartService cartService, long userId,
        CancellationToken cancellationToken)
    {
        Result<CartDto> result = await cartService.GetCartAsync(userId, cancellationToken);
        return result.ToHttpResponse();
    }
}