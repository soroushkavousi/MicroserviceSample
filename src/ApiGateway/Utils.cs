using System.Net;
using Company.Shared.Clients.ProductService.Models;
using Company.Shared.Clients.ProductService.Protos;

namespace ApiGateway;

public static class Utils
{
    public static int ResolveHttpStatusCode(this ProductErrorCodeView errorCode) => errorCode switch
    {
        ProductErrorCodeView.InternalServerError => (int)HttpStatusCode.InternalServerError,
        ProductErrorCodeView.AuthenticationError => (int)HttpStatusCode.Unauthorized,
        ProductErrorCodeView.AccessDenied => (int)HttpStatusCode.Forbidden,
        ProductErrorCodeView.ItemNotFound => (int)HttpStatusCode.NotFound,
        ProductErrorCodeView.ItemAlreadyExists
            or ProductErrorCodeView.InvalidFormat
            or ProductErrorCodeView.InvalidValue
            => (int)HttpStatusCode.BadRequest,
        _ => throw new ArgumentOutOfRangeException()
    };

    public static IResult ToApiErrorResponse<T>(this Result<T> result)
        => TypedResults.Json(result, statusCode: result.Error!.Value.ResolveHttpStatusCode());

    public static IResult ToApiErrorResponse(this Result result)
        => TypedResults.Json(result, statusCode: result.Error!.Value.ResolveHttpStatusCode());
}