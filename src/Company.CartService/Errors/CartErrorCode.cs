namespace Company.CartService.Errors;

public static class CartErrorCode
{
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string CartInvalidQuantity = "CART_INVALID_QUANTITY";
    public const string CartLineNotFound = "CART_LINE_NOT_FOUND";
    public const string ProductNotFound = "PRODUCT_NOT_FOUND";
}