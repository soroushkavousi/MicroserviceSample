using System.Collections.Concurrent;
using Company.ProductService.Models.Entities;

namespace Company.ProductService.Services;

public sealed class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<long, Product> _products = new();
    private long _lastId;

    public IEnumerable<Product> GetAll()
        => _products.Values;

    public bool TryGet(long id, out Product product)
        => _products.TryGetValue(id, out product);

    public long NextId()
        => Interlocked.Increment(ref _lastId);

    public bool Add(long id, Product product)
        => _products.TryAdd(id, product);

    public bool Remove(long id)
        => _products.TryRemove(id, out _);

    public bool NameExists(string name)
        => _products.Values.Any(p => p.Name == name);

    public bool NameExistsExcept(string name, long id)
        => _products.Values.Any(p => p.Name == name && p.Id != id);
}