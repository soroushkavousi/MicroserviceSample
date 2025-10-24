using Company.ProductService.Models;
using Company.Shared.Clients.ProductService.Protos;

namespace Company.ProductService.Mappers;

public static class ProductMapper
{
    public static readonly Func<Product, ProductView> ToView = p => new()
    {
        Id = p.Id,
        Name = p.Name,
        Price = p.Price,
        Description = p.Description
    };

    public static ProductView ToProductView(this Product product)
        => ToView(product);
}