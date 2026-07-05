using System.Net;

namespace Company.CartService.Errors;

public static class CartErrorCodeExtensions
{
    public static int ResolveHttpStatusCode(this string code)
        => code switch
        {
            CartErrorCode.CartLineNotFound or CartErrorCode.ProductNotFound
                => (int)HttpStatusCode.NotFound,
            CartErrorCode.CartInvalidQuantity => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

    public static string GetDescription(this string code)
        => code switch
        {
            CartErrorCode.CartInvalidQuantity => "Quantity must be greater than zero.",
            CartErrorCode.CartLineNotFound => "Cart line not found.",
            CartErrorCode.ProductNotFound => "Product not found.",
            CartErrorCode.InternalServerError => "An internal server error occurred.",
            _ => "An unexpected error occurred."
        };
}