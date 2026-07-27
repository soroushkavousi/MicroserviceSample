using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace Company.Shared.Extensions;

public static class TelemetryExtensions
{
    /// <summary>
    ///     Registers OpenTelemetry metrics (ASP.NET Core, HttpClient, runtime, custom meters)
    ///     and a Prometheus exporter scraped at <c>/metrics</c>.
    /// </summary>
    public static void AddTelemetry(this IServiceCollection services, string serviceName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(serviceName))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter(serviceName)
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("System.Net.Http")
                    .AddPrometheusExporter();
            });
    }

    /// <summary>
    ///     Exposes Prometheus scrape endpoint at <c>/metrics</c>.
    ///     DisableHttpMetrics keeps scrape traffic out of HTTP request metrics.
    /// </summary>
    public static void MapMetrics(this WebApplication app)
    {
        app.MapPrometheusScrapingEndpoint()
            .DisableHttpMetrics();
    }
}