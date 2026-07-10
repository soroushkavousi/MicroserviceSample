namespace Company.Shared.ValueObjects;

public record Error(string Code, string Description = null)
{
    public string Description { get; private set; } = Description;

    public void SetDescription(string description) =>
        Description = description;
}