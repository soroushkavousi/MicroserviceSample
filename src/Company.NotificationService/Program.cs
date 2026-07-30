using System.Reflection;
using Company.NotificationService.Consumers;
using Company.NotificationService.Services;
using Company.Shared.Extensions;
using Company.Shared.ProductService.Events;
using Confluent.Kafka;
using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelemetry(NotificationMetrics.ServiceName);
builder.Services.AddSingleton<INotificationMetrics, NotificationMetrics>();

Assembly assembly = Assembly.GetExecutingAssembly();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.UsingInMemory((context, cfg) => { });

    x.AddRider(rider =>
    {
        rider.AddConsumers(assembly);

        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:29092");

            k.TopicEndpoint<ProductCreatedEvent>(
                topicName: "product-created-event",
                groupId: "notification-service-group",
                configure: e =>
                {
                    e.ConfigureConsumer<ProductCreatedEventConsumer>(context);

                    e.AutoOffsetReset = AutoOffsetReset.Earliest;
                    e.ConcurrentMessageLimit = 1;
                    e.CheckpointInterval = TimeSpan.FromSeconds(3);
                    e.CheckpointMessageCount = 5000;
                });
        });
    });
});

WebApplication app = builder.Build();

app.MapMetrics();
app.Run();