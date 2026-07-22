using Company.Shared.Dtos;
using Company.Shared.Mappers;
using Company.Shared.ProductService.Errors;
using Company.Shared.ProductService.Protos;
using Company.Shared.ValueObjects;
using DotNetPotion.AppEnvironmentPack;
using Grpc.Core;
using Grpc.Net.Client;

namespace Company.Shared.ProductService;

public sealed class ProductServiceClient : IProductServiceClient, IDisposable
{
    private readonly ProductGrpcContract.ProductGrpcContractClient _client;
    private readonly GrpcChannel _channel;

    public ProductServiceClient(string address)
    {
        GrpcChannelOptions options = new();

        if (!AppEnvironment.IsProduction)
        {
            options.HttpHandler = new SocketsHttpHandler
            {
                SslOptions = new()
                {
                    RemoteCertificateValidationCallback = static (_, _, _, _) => true
                }
            };
        }

        _channel = GrpcChannel.ForAddress(address, options);
        _client = new(_channel);
    }

    public async Task<Result<ProductContractDto[]>> ListProductsAsync(string phrase = null,
        int page = 1, int pageSize = 10)
        => await ExecutePagedRequestAsync(async () =>
        {
            ListProductsRequest req = new()
            {
                Phrase = phrase,
                Page = page,
                PageSize = pageSize
            };

            ProductListContractDto resp = await _client.ListProductsAsync(req);
            return (resp.Items.ToArray(), resp.Pagination.ToPagination());
        });

    public async Task<Result<ProductContractDto[]>> ListProductsByIdsAsync(long[] ids)
        => await ExecuteRequestAsync(async () =>
        {
            ListProductsByIdsRequest req = new();
            req.Ids.AddRange(ids);
            ProductByIdsListContractDto resp = await _client.ListProductsByIdsAsync(req);
            return resp.Items.ToArray();
        });

    public async Task<Result<ProductContractDto>> GetProductAsync(long id)
        => await ExecuteRequestAsync(async () =>
        {
            ProductContractDto product = await _client.GetProductAsync(new() { Id = id });
            return product;
        });

    public async Task<Result<ProductContractDto>> CreateProductAsync(string name, double price,
        string description)
        => await ExecuteRequestAsync(async () =>
        {
            CreateProductRequest req = new()
            {
                Name = name,
                Price = price,
                Description = description
            };
            return await _client.CreateProductAsync(req);
        });

    public async Task<Result<ProductContractDto>> UpdateProductAsync(long id, string name,
        double price, string description)
        => await ExecuteRequestAsync(async () =>
        {
            UpdateProductRequest req = new()
            {
                Id = id,
                Name = name,
                Price = price,
                Description = description
            };
            return await _client.UpdateProductAsync(req);
        });

    public async Task<Result> DeleteProductAsync(long id)
        => await ExecuteRequestAsync(async () =>
        {
            await _client.DeleteProductAsync(new() { Id = id });
        });

    private static async Task<Result<T>> ExecutePagedRequestAsync<T>(
        Func<Task<(T Items, Pagination Pagination)>> request)
    {
        try
        {
            (T items, Pagination pagination) = await request();
            return (items, pagination);
        }
        catch (RpcException ex)
        {
            ErrorDto error = ex.GetRpcStatus()?.GetDetail<ErrorDto>();
            return error?.Code ?? ProductErrorCode.InternalServerError;
        }
        catch (Exception)
        {
            return ProductErrorCode.InternalServerError;
        }
    }

    private static async Task<Result<T>> ExecuteRequestAsync<T>(Func<Task<T>> request)
    {
        try
        {
            return await request();
        }
        catch (RpcException ex)
        {
            ErrorDto error = ex.GetRpcStatus()?.GetDetail<ErrorDto>();
            return error?.Code ?? ProductErrorCode.InternalServerError;
        }
        catch (Exception)
        {
            return ProductErrorCode.InternalServerError;
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
            ErrorDto error = ex.GetRpcStatus()?.GetDetail<ErrorDto>();
            return error?.Code ?? ProductErrorCode.InternalServerError;
        }
        catch (Exception)
        {
            return ProductErrorCode.InternalServerError;
        }
    }

    public void Dispose() => _channel.Dispose();
}