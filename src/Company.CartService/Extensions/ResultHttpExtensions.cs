using Company.CartService.Errors;
using Company.Shared.ValueObjects;

namespace Company.CartService.Extensions;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResponse(this Result result)
    {
        ApplyDefaultErrorDescription(result);
        return TypedResults.Json(result, statusCode: ResolveStatusCode(result));
    }

    public static IResult ToHttpResponse<TData>(this Result<TData> result)
    {
        ApplyDefaultErrorDescription(result);
        return TypedResults.Json(result, statusCode: ResolveStatusCode(result));
    }

    private static void ApplyDefaultErrorDescription(Result result)
    {
        if (result.HasError && string.IsNullOrWhiteSpace(result.Error.Description))
            result.SetErrorDescription(result.Error.Code.GetDescription());
    }

    private static int ResolveStatusCode(Result result)
        => result.HasError
            ? result.Error.Code.ResolveHttpStatusCode()
            : StatusCodes.Status200OK;
}