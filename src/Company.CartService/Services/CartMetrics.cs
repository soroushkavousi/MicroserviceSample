using System.Diagnostics.Metrics;

namespace Company.CartService.Services;

public sealed class CartMetrics(IMeterFactory meterFactory) : ICartMetrics
{
    public const string ServiceName = "cart-service";

    private readonly Counter<long> _itemsAdded =
        meterFactory.Create(ServiceName).CreateCounter<long>("cart.items.added", unit: "{item}",
            description: "Number of successful add-item operations.");

    private readonly Counter<long> _cartsViewed =
        meterFactory.Create(ServiceName).CreateCounter<long>("cart.views", unit: "{view}",
            description: "Number of successful cart views.");

    public void ItemAdded()
        => _itemsAdded.Add(1);

    public void CartViewed()
        => _cartsViewed.Add(1);
}
