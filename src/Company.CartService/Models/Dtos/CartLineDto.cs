namespace Company.CartService.Models.Dtos;

public sealed class CartLineDto
{
    public long ProductId { get; init; }
    public string ProductName { get; init; }
    public double UnitPrice { get; init; }
    public int Quantity { get; init; }
    public double LineTotal => UnitPrice * Quantity;
}