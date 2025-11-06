using Company.Shared.Clients.ProductService.Protos;
using Company.Shared.ValueObjects;
using Pagination = Company.Shared.ValueObjects.Pagination;

namespace Company.Shared.Clients.ProductService.Models;

public record Result : BaseResult<ProductErrorCodeView?>
{
    public Result() { }

    public Result(ProductErrorCodeView? errorCode) : base(errorCode) { }
}

public record Result<TData> : BaseResult<TData, ProductErrorCodeView?>
{
    public Result(TData data) : base(data) { }
    public Result(TData data, Pagination pagination) : base(data, pagination) { }
    public Result(ProductErrorCodeView errorCode) : base(errorCode) { }

    public static implicit operator Result<TData>(TData data) => new(data);
    public static implicit operator Result<TData>(ProductErrorCodeView errorCode) => new(errorCode);

    public static implicit operator Result<TData>((TData data, Pagination pagination) page) =>
        new(page.data, page.pagination);
}