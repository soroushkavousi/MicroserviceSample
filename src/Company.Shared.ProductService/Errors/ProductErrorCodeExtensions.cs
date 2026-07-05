using System.Net;

namespace Company.Shared.ProductService.Errors;

public static class ProductErrorCodeExtensions
{
    public static int ResolveHttpStatusCode(this string code)
        => code switch
        {
            ProductErrorCode.AuthenticationError => (int)HttpStatusCode.Unauthorized,
            ProductErrorCode.AccessDenied => (int)HttpStatusCode.Forbidden,
            ProductErrorCode.ProductNotFound => (int)HttpStatusCode.NotFound,
            ProductErrorCode.ProductAlreadyExists
                or ProductErrorCode.ProductInvalidFormat
                or ProductErrorCode.ProductInvalidValue
                => (int)HttpStatusCode.BadRequest,
            ProductErrorCode.InternalServerError => (int)HttpStatusCode.InternalServerError,
            _ => (int)HttpStatusCode.InternalServerError
        };

    public static string GetDescription(this string code)
        => code switch
        {
            ProductErrorCode.AuthenticationError => "Authentication failed.",
            ProductErrorCode.AccessDenied => "Access denied.",
            ProductErrorCode.ProductNotFound => "Product not found.",
            ProductErrorCode.ProductAlreadyExists => "A product with this name already exists.",
            ProductErrorCode.ProductInvalidFormat => "Product data format is invalid.",
            ProductErrorCode.ProductInvalidValue => "Product data value is invalid.",
            ProductErrorCode.InternalServerError => "An internal server error occurred.",
            _ => "An unexpected error occurred."
        };
}