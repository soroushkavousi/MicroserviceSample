using Company.Shared.ProductService.Events;
using MassTransit;
using NotificationService.Services;

namespace NotificationService.Consumers;

public class ProductCreatedEventConsumer(INotificationMetrics notificationMetrics,
    ILogger<ProductCreatedEventConsumer> logger) : IConsumer<ProductCreatedEvent>
{
    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        try
        {
            ProductCreatedEvent message = context.Message;

            logger.LogInformation("Product created event processed. ProductId={ProductId}, Name={Name},"
                + " Price={Price}", message.ProductId, message.Name, message.Price);

            notificationMetrics.ProductCreatedHandled();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process ProductCreatedEvent.");
            throw;
        }
    }
}