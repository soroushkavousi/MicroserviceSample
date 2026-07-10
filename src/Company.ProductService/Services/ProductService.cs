using Company.ProductService.Mappers;
using Company.ProductService.Models.Dtos;
using Company.ProductService.Models.Entities;
using Company.Shared.Extensions;
using Company.Shared.ProductService.Errors;
using Company.Shared.ProductService.Events;
using Company.Shared.ValueObjects;
using MassTransit;

namespace Company.ProductService.Services;

public sealed class ProductService(
    IProductRepository productRepository,
    ITopicProducer<long, ProductCreatedEvent> producer)
    : IProductService
{
    private readonly TimeSpan _processingDelay = TimeSpan.FromMilliseconds(15);

    public async Task<Result<ProductDto[]>> ListProductsAsync(string phrase, int page,
        int pageSize, CancellationToken cancellationToken)
    {
        int resolvedPage = page <= 0 ? 1 : page;
        int resolvedPageSize = pageSize <= 0 ? 10 : pageSize;

        IEnumerable<Product> query = productRepository.GetAll()
            .WhereIf(!string.IsNullOrWhiteSpace(phrase),
                x => x.Name.Contains(phrase!, StringComparison.OrdinalIgnoreCase)
                    || (x.Description ?? string.Empty).Contains(phrase, StringComparison.OrdinalIgnoreCase)
                    || x.Price.ToString().Contains(phrase, StringComparison.OrdinalIgnoreCase));

        int totalItems = query.Count();
        ProductDto[] items = query
            .Skip((resolvedPage - 1) * resolvedPageSize)
            .Take(resolvedPageSize)
            .Select(x => x.ToDto())
            .ToArray();

        Pagination pagination = new(resolvedPage, resolvedPageSize, totalItems);
        await Task.Delay(_processingDelay, cancellationToken);
        return (items, pagination);
    }

    public async Task<Result<ProductDto[]>> ListProductsByIdsAsync(long[] ids,
        CancellationToken cancellationToken)
    {
        if (ids.Length == 0)
            return Array.Empty<ProductDto>();

        List<ProductDto> products = [];
        foreach (long id in ids.Distinct())
        {
            if (productRepository.TryGet(id, out Product product))
                products.Add(product.ToDto());
        }

        await Task.Delay(_processingDelay, cancellationToken);
        return products.ToArray();
    }

    public async Task<Result<ProductDto>> GetProductAsync(long id,
        CancellationToken cancellationToken)
    {
        if (!productRepository.TryGet(id, out Product product))
            return ProductErrorCode.ProductNotFound;

        await Task.Delay(_processingDelay, cancellationToken);
        return product.ToDto();
    }

    public async Task<Result<ProductDto>> CreateProductAsync(
        string name, double price, string description, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ProductErrorCode.ProductInvalidFormat;

        if (productRepository.NameExists(name))
            return ProductErrorCode.ProductAlreadyExists;

        long id = productRepository.NextId();
        Product product = new(id, name, price, description);

        if (!productRepository.Add(id, product))
            return ProductErrorCode.InternalServerError;

        await Task.Delay(_processingDelay, cancellationToken);

        ProductCreatedEvent @event = new()
        {
            ProductId = id,
            Name = name,
            Price = (decimal)price,
            CreatedAt = DateTime.UtcNow
        };

        await producer.Produce(id, @event, cancellationToken);
        return product.ToDto();
    }

    public async Task<Result<ProductDto>> UpdateProductAsync(
        long id, string name, double price, string description,
        CancellationToken cancellationToken)
    {
        if (!productRepository.TryGet(id, out Product product))
            return ProductErrorCode.ProductNotFound;

        if (string.IsNullOrWhiteSpace(name))
            return ProductErrorCode.ProductInvalidFormat;

        if (name != product.Name && productRepository.NameExistsExcept(name, id))
            return ProductErrorCode.ProductAlreadyExists;

        product.Modify(name, price, description);
        await Task.Delay(_processingDelay, cancellationToken);
        return product.ToDto();
    }

    public async Task<Result> DeleteProductAsync(long id, CancellationToken cancellationToken)
    {
        if (!productRepository.Remove(id))
            return ProductErrorCode.ProductNotFound;

        await Task.Delay(_processingDelay, cancellationToken);
        return new();
    }
}