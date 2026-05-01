using Company.ProductService;
using Company.Services.Product.Events;
using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });

    x.AddRider(rider =>
    {
        rider.AddProducer<int, ProductCreatedEvent>("product-created");

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