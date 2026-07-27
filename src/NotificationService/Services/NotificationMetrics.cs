using System.Diagnostics.Metrics;

namespace NotificationService.Services;

public sealed class NotificationMetrics(IMeterFactory meterFactory) : INotificationMetrics
{
    public const string ServiceName = "notification-service";

    private readonly Counter<long> _productCreatedHandled =
        meterFactory.Create(ServiceName).CreateCounter<long>("notifications.product_created.handled",
            unit: "{event}", description: "Number of ProductCreatedEvent messages handled successfully.");

    public void ProductCreatedHandled()
        => _productCreatedHandled.Add(1);
}
