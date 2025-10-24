using System.Collections.Concurrent;
using Company.ProductService.Mappers;
using Company.ProductService.Models;
using Company.Shared.Clients.ProductService.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Company.ProductService;

public class ProductServiceGrpc : Shared.Clients.ProductService.Protos.ProductService.ProductServiceBase
{
    private static readonly ConcurrentDictionary<int, Product> _products = new();
    private static int _lastId;
    private static readonly TimeSpan _fakeDelay = TimeSpan.FromMilliseconds(15);

    public override async Task<ProductView> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        if (!_products.TryGetValue(request.Id, out Product product))
            throw new RpcException(new(StatusCode.NotFound, $"Product {request.Id} not found."));

        await Task.Delay(_fakeDelay);
        return product.ToProductView();
    }

    public override async Task<ProductView> CreateProduct(CreateProductRequest request, ServerCallContext context)
    {
        int productId = Interlocked.Increment(ref _lastId);
        Product product = new(productId, request.Name, request.Price, request.Description);
        bool itemAdded = _products.TryAdd(product.Id, product);
        if (!itemAdded)
            throw new Exception($"Product can not be added - {product}");

        await Task.Delay(_fakeDelay);
        return product.ToProductView();
    }

    public override async Task<ProductView> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
    {
        Product product = _products.AddOrUpdate(
            request.Id,
            _ => throw new RpcException(new(StatusCode.NotFound, $"Product {request.Id} not found.")),
            (_, product) =>
            {
                product.Modify(request.Name, request.Price, request.Description);
                return product;
            }
        );

        await Task.Delay(_fakeDelay);
        return product.ToProductView();
    }

    public override async Task<Empty> DeleteProduct(DeleteProductRequest request, ServerCallContext context)
    {
        if (!_products.TryRemove(request.Id, out _))
            throw new RpcException(new(StatusCode.NotFound, $"Product {request.Id} not found."));

        await Task.Delay(_fakeDelay);
        return new();
    }

    public override async Task<ProductListView> ListProducts(ListProductsRequest request, ServerCallContext context)
    {
        ProductListView response = new();
        response.Items.AddRange(_products.Values.Select(ProductMapper.ToView));

        await Task.Delay(_fakeDelay);
        return response;
    }
}