using Company.ProductService.Models;
using Company.Shared.Clients.ProductService.Protos;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Status = Google.Rpc.Status;

namespace Company.ProductService.Extensions;

public static class GrpcExtensions
{
    public static RpcException ToRpcException(this ProductError error)
    {
        StatusCode grpcCode = error.Code switch
        {
            ProductErrorCode.ItemNotFound => StatusCode.NotFound,
            ProductErrorCode.ItemAlreadyExists => StatusCode.AlreadyExists,
            ProductErrorCode.InvalidValue or ProductErrorCode.InvalidFormat
                => StatusCode.InvalidArgument,
            ProductErrorCode.AuthenticationError => StatusCode.Unauthenticated,
            ProductErrorCode.AccessDenied => StatusCode.PermissionDenied,
            _ => StatusCode.Internal
        };

        ProductErrorView errorView = new()
        {
            Code = (ProductErrorCodeView)(short)error.Code,
            Message = error.Message
        };

        return new Status
        {
            Code = (int)grpcCode,
            Message = error.Message,
            Details = { Any.Pack(errorView) }
        }.ToRpcException();
    }
}