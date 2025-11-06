using System.Collections.Concurrent;
using Company.ProductService.Extensions;
using Company.ProductService.Mappers;
using Company.ProductService.Models;
using Company.Shared.Clients.ProductService.Protos;
using Company.Shared.Extensions;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Company.ProductService;

public class ProductServiceGrpc : ProductGrpcContract.ProductGrpcContractBase
{
    private static readonly ConcurrentDictionary<int, Product> _products = new();
    private static int _lastId;
    private static readonly TimeSpan _fakeDelay = TimeSpan.FromMilliseconds(15);

    public override async Task<ProductListView> ListProducts(ListProductsRequest request, ServerCallContext context)
    {
        int page = request.Page <= 0 ? 1 : request.Page;
        int pageSize = request.PageSize <= 0 ? _products.Count : request.PageSize;

        ProductView[] products = _products.Values
            .WhereIf(!string.IsNullOrWhiteSpace(request.Phrase),
                x => x.Name.Contains(request.Phrase, StringComparison.OrdinalIgnoreCase))
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => p.ToProductView())
            .ToArray();

        await Task.Delay(_fakeDelay, context.CancellationToken);
        ProductListView result = new();
        result.Items.AddRange(products);
        return result;
    }

    public override async Task<ProductView> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        if (!_products.TryGetValue(request.Id, out Product product))
            throw new ProductError(ProductErrorCode.ItemNotFound, $"Product {request.Id} not found.")
                .ToRpcException();

        await Task.Delay(_fakeDelay, context.CancellationToken);
        return product.ToProductView();
    }

    public override async Task<ProductView> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ProductError(ProductErrorCode.InvalidFormat, "Product name is required.")
                .ToRpcException();

        bool exists = _products.Values.Any(p => p.Name == request.Name);
        if (exists)
            throw new ProductError(ProductErrorCode.ItemAlreadyExists, $"Product '{request.Name}' already exists.")
                .ToRpcException();

        int id = Interlocked.Increment(ref _lastId);
        Product product = new(id, request.Name, request.Price, request.Description);

        if (!_products.TryAdd(id, product))
            throw new ProductError(ProductErrorCode.InternalServerError, $"Failed to add product {request.Name}.")
                .ToRpcException();

        await Task.Delay(_fakeDelay, context.CancellationToken);
        return product.ToProductView();
    }

    public override async Task<ProductView> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
    {
        if (!_products.TryGetValue(request.Id, out Product product))
            throw new ProductError(ProductErrorCode.ItemNotFound, $"Product {request.Id} not found.")
                .ToRpcException();

        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ProductError(ProductErrorCode.InvalidFormat, "Product name cannot be empty.")
                .ToRpcException();

        if (request.Name != product.Name)
        {
            bool exists = _products.Values.Any(p => p.Name == request.Name);
            if (exists)
                throw new ProductError(ProductErrorCode.ItemAlreadyExists, $"Product '{request.Name}' already exists.")
                    .ToRpcException();
        }

        product.Modify(request.Name, request.Price, request.Description);
        await Task.Delay(_fakeDelay, context.CancellationToken);

        return product.ToProductView();
    }

    public override async Task<Empty> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        if (!_products.TryRemove(request.Id, out _))
            throw new ProductError(ProductErrorCode.ItemNotFound, $"Product {request.Id} not found.")
                .ToRpcException();

        await Task.Delay(_fakeDelay, context.CancellationToken);
        return new();
    }
}