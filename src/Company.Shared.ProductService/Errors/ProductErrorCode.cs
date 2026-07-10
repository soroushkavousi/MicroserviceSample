namespace Company.Shared.ProductService.Errors;

public static class ProductErrorCode
{
    public const string InternalServerError = "INTERNAL_SERVER_ERROR";
    public const string AuthenticationError = "AUTHENTICATION_ERROR";
    public const string AccessDenied = "ACCESS_DENIED";
    public const string ProductNotFound = "PRODUCT_NOT_FOUND";
    public const string ProductAlreadyExists = "PRODUCT_ALREADY_EXISTS";
    public const string ProductInvalidFormat = "PRODUCT_INVALID_FORMAT";
    public const string ProductInvalidValue = "PRODUCT_INVALID_VALUE";
}