using System.Diagnostics.Metrics;

namespace Company.ProductService.Services;

public sealed class ProductMetrics(IMeterFactory meterFactory) : IProductMetrics
{
    public const string ServiceName = "product-service";

    private readonly Counter<long> _productsCreated =
        meterFactory.Create(ServiceName).CreateCounter<long>("products.created", unit: "{product}",
            description: "Number of products created successfully.");

    public void ProductCreated()
        => _productsCreated.Add(1);
}
