using Company.Shared.ValueObjects;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Company.Shared.Extensions;

public static class OpenApiOptionsExtensions
{
    private static readonly Dictionary<Type, IOpenApiAny> SchemaExamples = new()
    {
        [typeof(Pagination)] = new OpenApiObject
        {
            ["pageNumber"] = new OpenApiInteger(1),
            ["pageSize"] = new OpenApiInteger(10),
            ["totalItems"] = new OpenApiInteger(42),
            ["totalPages"] = new OpenApiInteger(5)
        }
    };

    public static OpenApiOptions AddSharedOpenApiExamples(this OpenApiOptions options)
    {
        options.AddOperationTransformer((operation, _, _) =>
        {
            TrySetFailureExample(operation, "400", "BAD_REQUEST", "bad request");
            TrySetFailureExample(operation, "401", "UNAUTHORIZED", "unauthorized");
            TrySetFailureExample(operation, "404", "NOT_FOUND", "not found");
            return Task.CompletedTask;
        });

        options.AddSchemaTransformer((schema, context, _) =>
        {
            if (SchemaExamples.TryGetValue(context.JsonTypeInfo.Type, out IOpenApiAny example))
                schema.Example = example;
            return Task.CompletedTask;
        });

        return options;
    }

    private static void TrySetFailureExample(
        OpenApiOperation operation, string statusCode, string errorCode, string errorMessage)
    {
        if (operation.Responses is null
            || !operation.Responses.TryGetValue(statusCode, out OpenApiResponse response))
            return;

        if (response.Content is null
            || !response.Content.TryGetValue("application/json", out OpenApiMediaType mediaType))
            return;

        mediaType.Example = new OpenApiObject
        {
            ["error"] = new OpenApiObject
            {
                ["code"] = new OpenApiString(errorCode),
                ["description"] = new OpenApiString(errorMessage)
            },
            ["isSuccess"] = new OpenApiBoolean(false)
        };
    }
}