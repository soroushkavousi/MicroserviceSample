namespace Company.ProductService.Models.Dtos;

public sealed record ProductDto
{
    public long Id { get; init; }
    public string Name { get; init; }
    public double Price { get; init; }
    public string Description { get; init; }
}