using Microsoft.OpenApi.Models;

namespace Company.CartService.Extensions;

public static class OpenApiExtensions
{
    public static void AddCartServiceOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
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
                document.Components ??= new();
                document.Components.SecuritySchemes["Bearer"] = new()
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "userId",
                    Description = "Fake JWT used by ApiGateway: pass a numeric user id, e.g. Bearer 42."
                };
                document.SecurityRequirements.Add(new()
                {
                    [new()
                    {
                        Reference = new()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }] = []
                });
                return Task.CompletedTask;
            });
        });
    }
}