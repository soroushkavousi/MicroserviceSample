using Company.Shared.ValueObjects;

namespace Company.ProductService.Models;

public record ProductError : BaseError<ProductErrorCode>
{
    public ProductError(ProductErrorCode code) : base(code)
    {
    }

    public ProductError(ProductErrorCode code, string message) : base(code, message)
    {
    }
}