using Company.Shared.ValueObjects;

namespace Company.ProductService.Models;

public record Result : BaseResult<ProductErrorCode>
{
    protected Result() { }

    public Result(ProductErrorCode errorCode) : base(errorCode) { }
}

public record Result<TData> : BaseResult<TData, ProductErrorCode>
{
    public Result(TData data) : base(data) { }
    public Result(TData data, Pagination pagination) : base(data, pagination) { }
    public Result(ProductErrorCode errorCode) : base(errorCode) { }

    public static implicit operator Result<TData>(TData data) => new(data);
    public static implicit operator Result<TData>(ProductErrorCode productErrorCode) => new(productErrorCode);

    public static implicit operator Result<TData>((TData data, Pagination pagination) page) =>
        new(page.data, page.pagination);
}