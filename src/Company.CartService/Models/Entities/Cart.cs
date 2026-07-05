namespace Company.CartService.Models.Entities;

public sealed class Cart
{
    private Cart() { }

    public Cart(long userId)
    {
        UserId = userId;
    }

    public long UserId { get; }

    private readonly List<CartLine> _lines = [];
    public IReadOnlyCollection<CartLine> Lines => _lines;

    public void AddItem(long productId, int quantity)
    {
        CartLine existingLine = _lines.FirstOrDefault(x => x.ProductId == productId);
        if (existingLine is null)
            _lines.Add(new(productId, quantity));
        else
            existingLine.AddQuantity(quantity);
    }

    public bool RemoveItem(long productId)
    {
        CartLine line = _lines.FirstOrDefault(x => x.ProductId == productId);
        if (line is null)
            return false;

        _lines.Remove(line);
        return true;
    }

    public void Clear() => _lines.Clear();
}