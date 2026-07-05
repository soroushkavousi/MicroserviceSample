namespace Company.CartService.Models.Dtos;

public sealed class CartDto
{
    public long UserId { get; init; }
    public CartLineDto[] Lines { get; init; }
    public double Total { get; init; }
}