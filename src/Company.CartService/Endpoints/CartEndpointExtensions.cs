namespace Company.CartService.Endpoints;

public static class CartEndpointExtensions
{
    public static void MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        RouteGroupBuilder cart = app.MapGroup("/cart").WithTags("Cart");

        cart.MapGetCart();
        cart.MapAddCartItem();
        cart.MapRemoveCartItem();
        cart.MapClearCart();
    }
}