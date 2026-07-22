namespace Company.ProductService.Extensions;

public static class OpenApiExtensions
{
    public static void AddProductServiceOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new()
                {
                    Title = "Company.ProductService",
                    Version = "v1",
                    Description = "Product catalog REST API."
                };
                return Task.CompletedTask;
            });
        });
    }
}