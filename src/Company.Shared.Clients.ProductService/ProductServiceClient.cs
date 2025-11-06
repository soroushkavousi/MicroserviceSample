using Company.Shared.Clients.ProductService.Models;
using Company.Shared.Clients.ProductService.Protos;
using Grpc.Core;
using Grpc.Net.Client;

namespace Company.Shared.Clients.ProductService;

public sealed class ProductServiceClient : IProductServiceClient, IDisposable
{
    private readonly ProductGrpcContract.ProductGrpcContractClient _client;
    private readonly GrpcChannel _channel;

    public ProductServiceClient(string address)
    {
        _channel = GrpcChannel.ForAddress(address);
        _client = new(_channel);
    }

    public async Task<Result<ProductView[]>> ListProductsAsync(string phrase = null,
        int page = 1, int pageSize = 10)
        => await ExecuteRequestAsync(async () =>
        {
            ListProductsRequest req = new()
            {
                Phrase = phrase,
                Page = page,
                PageSize = pageSize
            };

            ProductListView resp = await _client.ListProductsAsync(req);
            return resp.Items.ToArray();
        });

    public async Task<Result<ProductView>> GetProductAsync(int id)
        => await ExecuteRequestAsync(async () =>
        {
            ProductView product = await _client.GetProductAsync(new() { Id = id });
            return product;
        });

    public async Task<Result<ProductView>> CreateProductAsync(string name, double price, string description)
        => await ExecuteRequestAsync(async () =>
        {
            CreateProductRequest req = new() { Name = name, Price = price, Description = description };
            return await _client.CreateProductAsync(req);
        });

    public async Task<Result<ProductView>> UpdateProductAsync(int id, string name, double price, string description)
        => await ExecuteRequestAsync(async () =>
        {
            UpdateProductRequest req = new() { Id = id, Name = name, Price = price, Description = description };
            return await _client.UpdateProductAsync(req);
        });

    public async Task<Result> DeleteProductAsync(int id)
        => await ExecuteRequestAsync(async () =>
        {
            await _client.DeleteProductAsync(new() { Id = id });
        });

    private static async Task<Result<T>> ExecuteRequestAsync<T>(Func<Task<T>> request)
    {
        try
        {
            return await request();
        }
        catch (RpcException ex)
        {
            ProductErrorView error = ex.GetRpcStatus()?.GetDetail<ProductErrorView>();
            return error?.Code ?? ProductErrorCodeView.InternalServerError;
        }
        catch (Exception ex)
        {
            return new(ProductErrorCodeView.InternalServerError);
        }
    }

    private static async Task<Result> ExecuteRequestAsync(Func<Task> request)
    {
        try
        {
            await request();
            return new();
        }
        catch (RpcException ex)
        {
            ProductErrorView error = ex.GetRpcStatus()?.GetDetail<ProductErrorView>();
            return error is not null ? new(error.Code) : new Result(ProductErrorCodeView.InternalServerError);
        }
        catch (Exception ex)
        {
            return new(ProductErrorCodeView.InternalServerError);
        }
    }

    public void Dispose() => _channel.Dispose();
}