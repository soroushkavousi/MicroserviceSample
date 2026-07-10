namespace Company.Shared.ProductService.Events;

public record ProductCreatedEvent
{
    public long ProductId { get; init; }
    public string Name { get; init; }
    public decimal Price { get; init; }
    public DateTime CreatedAt { get; init; }
}