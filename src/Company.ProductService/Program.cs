using System.Reflection;
using Company.ProductService;
using Company.ProductService.Endpoints;
using Company.ProductService.Services;
using Company.Shared.Extensions;
using Company.Shared.ProductService.Events;
using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureSharedHttpJsonOptions();
builder.Services.AddGrpc();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductServiceGrpc>();

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
        rider.AddProducer<long, ProductCreatedEvent>("product-created-event");

        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:29092");
        });
    });
});

WebApplication app = builder.Build();

app.MapGrpcService<ProductServiceGrpc>();
app.MapProductEndpoints();
app.MapGet("/", () => Results.Ok(new { service = "Company.ProductService" }));

app.Run();