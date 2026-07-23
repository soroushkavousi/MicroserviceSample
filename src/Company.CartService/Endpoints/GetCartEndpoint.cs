using Company.CartService.Extensions;
using Company.CartService.Models.Dtos;
using Company.CartService.Services;
using Company.Shared.Extensions;
using Company.Shared.ValueObjects;

namespace Company.CartService.Endpoints;

public static class GetCartEndpoint
{
    public static void MapGetCart(this RouteGroupBuilder group)
    {
        group.MapGet("/", HandleAsync)
            .WithName("GetCart")
            .WithSummary("Get cart")
            .WithDescription("Returns the current user's shopping cart.");
    }

    private static async Task<IResult> HandleAsync(
        HttpContext httpContext, ICartService cartService, CancellationToken cancellationToken)
    {
        if (!httpContext.TryGetUserId(out long userId))
            return Results.Unauthorized();

        Result<CartDto> result = await cartService.GetCartAsync(userId, cancellationToken);
        return result.ToHttpResponse();
    }
}