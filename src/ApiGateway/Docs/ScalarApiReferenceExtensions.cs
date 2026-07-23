using DotNetPotion.AppEnvironmentPack;
using Scalar.AspNetCore;

namespace ApiGateway.Docs;

public static class ScalarApiReferenceExtensions
{
    public const string RoutePrefix = "docs";

    public static void MapApiDocs(this WebApplication app)
    {
        app.MapScalarApiReference(RoutePrefix, (options, httpContext) =>
        {
            string gatewayBase = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

            options.WithTitle("Company API Gateway")
                .WithDefaultHttpClient(ScalarTarget.JavaScript, ScalarClient.Fetch)
                .ExpandAllTags()
                .AddServer(gatewayBase, "Gateway")
                .AddDocument("products", "Product Service", routePattern: "/openapi/products.json")
                .AddDocument("cart", "Cart Service", routePattern: "/openapi/cart.json", isDefault: true)
                .AddPreferredSecuritySchemes("Bearer");

            if (!AppEnvironment.IsProduction)
            {
                options.AddHttpAuthentication("Bearer", auth => auth.Token = "1");
            }
        });
    }
}