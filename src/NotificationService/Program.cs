using Company.Services.Product.Events;
using Confluent.Kafka;
using MassTransit;
using NotificationService.Consumers;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ProductCreatedConsumer>();

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddRider(rider =>
    {
        rider.AddConsumer<ProductCreatedConsumer>();

        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:29092");

            k.TopicEndpoint<ProductCreatedEvent>(
                topicName: "product-created",
                groupId: "notification-service-group",
                configure: e =>
                {
                    e.AutoOffsetReset = AutoOffsetReset.Earliest;

                    e.ConcurrentMessageLimit = 1;

                    e.CheckpointInterval = TimeSpan.FromSeconds(3);
                    e.CheckpointMessageCount = 5000;

                    e.ConfigureConsumer<ProductCreatedConsumer>(context);
                });
        });
    });
});

WebApplication app = builder.Build();

Console.WriteLine("NotificationService is started.");
app.MapGet("/", () => "NotificationService is running...");

app.Run();