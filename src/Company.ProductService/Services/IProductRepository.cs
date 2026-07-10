using Company.ProductService.Models.Entities;

namespace Company.ProductService.Services;

public interface IProductRepository
{
    IEnumerable<Product> GetAll();

    bool TryGet(long id, out Product product);

    long NextId();

    bool Add(long id, Product product);

    bool Remove(long id);

    bool NameExists(string name);

    bool NameExistsExcept(string name, long id);
}