using Company.ProductService.Extensions;
using Company.ProductService.Mappers;
using Company.ProductService.Models.Dtos;
using Company.ProductService.Services;
using Company.Shared.Mappers;
using Company.Shared.ProductService.Protos;
using Company.Shared.ValueObjects;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Company.ProductService;

public class ProductServiceGrpc(IProductService productService)
    : ProductGrpcContract.ProductGrpcContractBase
{
    public override async Task<ProductListContractDto> ListProducts(ListProductsRequest request,
        ServerCallContext context)
    {
        Result<ProductDto[]> listResult = await productService.ListProductsAsync(
            request.Phrase, request.Page, request.PageSize, context.CancellationToken);
        if (listResult.HasError)
            throw listResult.Error.ToRpcException();

        ProductListContractDto result = new();
        result.Items.AddRange(listResult.Data.Select(x => x.ToContractDto()));
        result.Pagination = listResult.Pagination.ToDto();
        return result;
    }

    public override async Task<ProductByIdsListContractDto> ListProductsByIds(
        ListProductsByIdsRequest request, ServerCallContext context)
    {
        Result<ProductDto[]> listResult = await productService.ListProductsByIdsAsync(
            request.Ids.ToArray(), context.CancellationToken);
        if (listResult.HasError)
            throw listResult.Error.ToRpcException();

        ProductByIdsListContractDto result = new();
        result.Items.AddRange(listResult.Data.Select(x => x.ToContractDto()));
        return result;
    }

    public override async Task<ProductContractDto> GetProduct(GetProductRequest request,
        ServerCallContext context)
    {
        Result<ProductDto> result = await productService.GetProductAsync(
            request.Id, context.CancellationToken);
        if (result.HasError)
            throw result.Error.ToRpcException();

        return result.Data.ToContractDto();
    }

    public override async Task<ProductContractDto> CreateProduct(CreateProductRequest request,
        ServerCallContext context)
    {
        Result<ProductDto> result = await productService.CreateProductAsync(
            request.Name, request.Price, request.Description, context.CancellationToken);
        if (result.HasError)
            throw result.Error.ToRpcException();

        return result.Data.ToContractDto();
    }

    public override async Task<ProductContractDto> UpdateProduct(UpdateProductRequest request,
        ServerCallContext context)
    {
        Result<ProductDto> result = await productService.UpdateProductAsync(
            request.Id, request.Name, request.Price, request.Description,
            context.CancellationToken);
        if (result.HasError)
            throw result.Error.ToRpcException();

        return result.Data.ToContractDto();
    }

    public override async Task<Empty> DeleteProduct(DeleteProductRequest request,
        ServerCallContext context)
    {
        Result result = await productService.DeleteProductAsync(
            request.Id, context.CancellationToken);
        if (result.HasError)
            throw result.Error.ToRpcException();

        return new();
    }
}