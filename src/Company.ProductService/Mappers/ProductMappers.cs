using Company.ProductService.Models.Dtos;
using Company.ProductService.Models.Entities;
using Company.Shared.ProductService.Protos;

namespace Company.ProductService.Mappers;

public static class ProductMappers
{
    public static ProductDto ToDto(this Product product) => new()
    {
        Id = product.Id,
        Name = product.Name,
        Price = product.Price,
        Description = product.Description
    };

    public static ProductContractDto ToContractDto(this ProductDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Price = dto.Price,
        Description = dto.Description
    };
}