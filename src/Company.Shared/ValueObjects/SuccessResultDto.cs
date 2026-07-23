using System.ComponentModel;

namespace Company.Shared.ValueObjects;

/// <summary>OpenAPI success envelope (200) without error or pagination.</summary>
public sealed record SuccessResultDto
{
    [DefaultValue(true)]
    public bool IsSuccess { get; init; }
}

/// <summary>OpenAPI success envelope (200) with data only.</summary>
public sealed record SuccessResultDto<TData>
{
    public TData Data { get; init; }

    [DefaultValue(true)]
    public bool IsSuccess { get; init; }
}