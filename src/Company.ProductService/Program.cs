using System.Reflection;
using Company.ProductService;
using Company.Services.Product.Events;
using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

Assembly assembly = Assembly.GetExecutingAssembly();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumers(assembly);

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddRider(rider =>
    {
        rider.AddProducer<int, ProductCreatedEvent>("product-created-event");

        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:29092");
        });
    });
});

WebApplication app = builder.Build();

app.MapGrpcService<ProductServiceGrpc>();
app.MapGet("/", () => "ProductService is running...");

app.Run();