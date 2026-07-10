namespace Company.CartService.Models.Entities;

public sealed class CartLine
{
    private CartLine() { }

    public CartLine(long productId, int quantity)
    {
        ProductId = productId;
        Quantity = quantity;
    }

    public long ProductId { get; }
    public int Quantity { get; private set; }

    public void AddQuantity(int quantity) => Quantity += quantity;
}