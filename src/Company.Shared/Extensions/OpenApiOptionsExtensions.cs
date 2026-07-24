using System.Text.Json.Nodes;
using Company.Shared.ValueObjects;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Company.Shared.Extensions;

public static class OpenApiOptionsExtensions
{
    private static readonly Dictionary<Type, JsonNode> _schemaExamples = new()
    {
        [typeof(Pagination)] = new JsonObject
        {
            ["pageNumber"] = 1,
            ["pageSize"] = 10,
            ["totalItems"] = 42,
            ["totalPages"] = 5
        }
    };

    public static void AddSharedOpenApiExamples(this OpenApiOptions options)
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
            if (_schemaExamples.TryGetValue(context.JsonTypeInfo.Type, out JsonNode example))
                schema.Example = example.DeepClone();
            return Task.CompletedTask;
        });
    }

    private static void TrySetFailureExample(
        OpenApiOperation operation, string statusCode, string errorCode, string errorMessage)
    {
        if (operation.Responses is null
            || !operation.Responses.TryGetValue(statusCode, out IOpenApiResponse response)
            || response.Content is null
            || !response.Content.TryGetValue("application/json", out OpenApiMediaType mediaType))
            return;

        mediaType.Example = new JsonObject
        {
            ["error"] = new JsonObject
            {
                ["code"] = errorCode,
                ["description"] = errorMessage
            },
            ["isSuccess"] = false
        };
    }
}