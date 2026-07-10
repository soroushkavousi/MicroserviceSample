using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Company.Shared.Extensions;

public static class JsonSerializerExtensions
{
    public static void ConfigureSharedHttpJsonOptions(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.ConfigureSharedDefaults();
        });
    }

    public static void ConfigureSharedDefaults(this JsonSerializerOptions serializer)
    {
        serializer.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    }
}
