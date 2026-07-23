using System.ComponentModel;

namespace Company.Shared.ValueObjects;

/// <summary>OpenAPI success envelope (200) for paginated list responses.</summary>
public sealed record PagedSuccessResultDto<TData>
{
    public TData Data { get; init; }
    public Pagination Pagination { get; init; }

    [DefaultValue(true)]
    public bool IsSuccess { get; init; }
}