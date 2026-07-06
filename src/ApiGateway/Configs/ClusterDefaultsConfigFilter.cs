using DotNetPotion.AppEnvironmentPack;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;

namespace ApiGateway.Configs;

public sealed class ClusterDefaultsConfigFilter : IProxyConfigFilter
{
    private static readonly ForwarderRequestConfig _http2Request = new()
    {
        Version = new(2, 0),
        VersionPolicy = HttpVersionPolicy.RequestVersionExact
    };

    public ValueTask<ClusterConfig> ConfigureClusterAsync(ClusterConfig cluster,
        CancellationToken cancel)
    {
        HttpClientConfig httpClient = cluster.HttpClient ?? new HttpClientConfig();

        if (AppEnvironment.IsDevelopment)
        {
            httpClient = httpClient with { DangerousAcceptAnyServerCertificate = true };
        }

        return new(cluster with
        {
            HttpRequest = _http2Request,
            HttpClient = httpClient
        });
    }

    public ValueTask<RouteConfig> ConfigureRouteAsync(RouteConfig route, ClusterConfig cluster,
        CancellationToken cancel)
        => new(route);
}