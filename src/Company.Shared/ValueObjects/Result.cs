using System.Text.Json.Serialization;
using Ardalis.GuardClauses;

namespace Company.Shared.ValueObjects;

public record Result
{
    public Result() { }

    public Result(Error error)
    {
        Guard.Against.Null(error);
        Error = error;
    }

    public Error Error { get; }
    public bool IsSuccess => Error is null;
    [JsonIgnore]
    public bool HasError => !IsSuccess;

    public static implicit operator Result(Error error) => new(error);
    public static implicit operator Result(string errorCode) => new(new(errorCode));

    public void SetErrorDescription(string errorDescription)
    {
        Guard.Against.NullOrWhiteSpace(errorDescription);
        Error.SetDescription(errorDescription);
    }
}

public record Result<TData> : Result
{
    public Result(TData data)
    {
        Guard.Against.Null(data);
        Data = data;
    }

    public Result(TData data, Pagination pagination)
        : this(data)
    {
        Pagination = pagination;
    }

    public Result(Error error)
        : base(error)
    {
    }

    public Result(string errorCode)
        : base(new(errorCode))
    {
    }

    public TData Data { get; init; }
    public Pagination Pagination { get; init; }

    public static implicit operator Result<TData>(TData data) => new(data);
    public static implicit operator Result<TData>(Error error) => new(error);
    public static implicit operator Result<TData>(string errorCode) => new(errorCode);

    public static implicit operator Result<TData>((TData data, Pagination pagination) page) =>
        new(page.data, page.pagination);
}