using Ardalis.GuardClauses;

namespace Company.Shared.ValueObjects;

public abstract record BaseError<TErrorCode>
    where TErrorCode : struct, Enum
{
    protected BaseError(TErrorCode code)
    {
        Code = Guard.Against.EnumOutOfRange(code);
        Timestamp = DateTime.UtcNow;
    }

    protected BaseError(TErrorCode code, string message)
        : this(code)
    {
        SetDetails(message);
    }

    public TErrorCode Code { get; }
    public string Message { get; private set; }
    public DateTime Timestamp { get; }

    public void SetDetails(string message)
    {
        Message = message;
    }
}