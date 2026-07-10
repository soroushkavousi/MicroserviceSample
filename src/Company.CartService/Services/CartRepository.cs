using System.Collections.Concurrent;
using Company.CartService.Models.Entities;

namespace Company.CartService.Services;

public sealed class CartRepository : ICartRepository
{
    private readonly ConcurrentDictionary<long, Cart> _carts = new();

    public Cart GetOrCreate(long userId)
        => _carts.GetOrAdd(userId, id => new(id));
}