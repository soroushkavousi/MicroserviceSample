namespace Company.Services.Product.Events;

public record ProductCreatedEvent
{
    public int ProductId { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
}