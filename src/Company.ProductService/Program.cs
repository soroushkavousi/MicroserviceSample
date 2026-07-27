using System.Reflection;
using Company.ProductService;
using Company.ProductService.Endpoints;
using Company.ProductService.Extensions;
using Company.ProductService.Services;
using Company.Shared.Extensions;
using Company.Shared.ProductService.Events;
using DotNetPotion.AppEnvironmentPack;
using MassTransit;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSharedHttpJsonOptions();
builder.Services.AddTelemetry(ProductMetrics.ServiceName);
builder.Services.AddSingleton<IProductMetrics, ProductMetrics>();
builder.Services.AddGrpc();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ProductServiceGrpc>();
builder.Services.AddProductServiceOpenApi();

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

if (!AppEnvironment.IsProduction)
    app.MapOpenApi();

app.MapMetrics();
app.MapGrpcService<ProductServiceGrpc>();
app.MapProductEndpoints();

app.Run();