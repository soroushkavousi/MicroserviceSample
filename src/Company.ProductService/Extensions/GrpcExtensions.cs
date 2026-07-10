using Company.Shared.ProductService.Errors;
using Company.Shared.Dtos;
using Company.Shared.ValueObjects;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Status = Google.Rpc.Status;

namespace Company.ProductService.Extensions;

public static class GrpcExtensions
{
    public static RpcException ToRpcException(this Error error)
    {
        StatusCode grpcCode = error.Code switch
        {
            ProductErrorCode.ProductNotFound => StatusCode.NotFound,
            ProductErrorCode.ProductAlreadyExists => StatusCode.AlreadyExists,
            ProductErrorCode.ProductInvalidValue or ProductErrorCode.ProductInvalidFormat
                => StatusCode.InvalidArgument,
            ProductErrorCode.AuthenticationError => StatusCode.Unauthenticated,
            ProductErrorCode.AccessDenied => StatusCode.PermissionDenied,
            _ => StatusCode.Internal
        };

        string description = string.IsNullOrWhiteSpace(error.Description)
            ? error.Code.GetDescription()
            : error.Description;

        ErrorDto errorDto = new()
        {
            Code = error.Code,
            Message = description
        };

        Status status = new()
        {
            Code = (int)grpcCode,
            Message = errorDto.Message,
            Details = { Any.Pack(errorDto) }
        };
        return status.ToRpcException();
    }
}