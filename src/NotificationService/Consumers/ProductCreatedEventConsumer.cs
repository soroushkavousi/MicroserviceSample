using Company.Services.Product.Events;
using MassTransit;

namespace NotificationService.Consumers;

public class ProductCreatedEventConsumer(ILogger<ProductCreatedEventConsumer> logger)
    : IConsumer<ProductCreatedEvent>
{
    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        try
        {
            ProductCreatedEvent message = context.Message;

            logger.LogInformation(
                "📧[{DateTime}] Email sent to admin: Product created " +
                "- {Name} ({Id}) Price: {Price}",
                DateTime.UtcNow,
                message.Name,
                message.ProductId,
                message.Price);

            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}