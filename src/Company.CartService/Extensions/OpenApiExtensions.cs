using Company.Shared.Extensions;
using Microsoft.OpenApi;

namespace Company.CartService.Extensions;

public static class OpenApiExtensions
{
    public static void AddCartServiceOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddSharedOpenApiExamples();
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "Company.CartService",
                    Version = "v1",
                    Description =
                        "Shopping cart REST API. Through the gateway, authorize with Bearer {userId} " +
                        "(numeric user id)."
                };
                document.Tags = new HashSet<OpenApiTag>
                {
                    new() { Name = "Cart", Description = "Shopping cart operations for the authenticated user." }
                };
                document.Components ??= new();
                document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "userId",
                    Description = "Fake JWT used by ApiGateway: pass a numeric user id, e.g. Bearer 42."
                };
                document.Security ??= [];
                document.Security.Add(new()
                {
                    [new("Bearer", document)] = []
                });
                return Task.CompletedTask;
            });
        });
    }
}