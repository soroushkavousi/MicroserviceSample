using System.Reflection;
using Company.Shared.ProductService.Events;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Options;

namespace Company.ProductService.Messaging;

public sealed class ProductKafkaTopicInitializer(
    IOptions<KafkaOptions> kafkaOptions,
    ILogger<ProductKafkaTopicInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        KafkaOptions options = kafkaOptions.Value;

        string[] topicNames = typeof(ProductKafkaTopics)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(field => field.IsLiteral && !field.IsInitOnly
                && field.FieldType == typeof(string))
            .Select(field => (string)field.GetRawConstantValue())
            .Distinct()
            .ToArray();

        if (topicNames.Length == 0)
            return;

        using IAdminClient admin = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = options.BootstrapServers
        }).Build();

        Metadata metadata = admin.GetMetadata(timeout: TimeSpan.FromSeconds(10));
        HashSet<string> existingTopics = metadata.Topics
            .Select(topic => topic.Topic)
            .ToHashSet(StringComparer.Ordinal);

        List<TopicSpecification> topicsToCreate = topicNames
            .Where(name => !existingTopics.Contains(name))
            .Select(name => BuildTopicSpecification(options, name))
            .ToList();

        if (topicsToCreate.Count == 0)
            return;

        try
        {
            await admin.CreateTopicsAsync(topicsToCreate);
            logger.LogInformation("Created Kafka topics: {Topics}",
                string.Join(", ", topicsToCreate.Select(topic => topic.Name)));
        }
        catch (CreateTopicsException ex) when (ex.Results.All(result =>
            result.Error.Code is ErrorCode.TopicAlreadyExists or ErrorCode.NoError))
        {
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    private static TopicSpecification BuildTopicSpecification(KafkaOptions options, string topicName)
    {
        KafkaTopicDefaults defaults = options.Defaults ?? new();
        Dictionary<string, KafkaTopicOverride> topics = options.Topics ?? new();
        topics.TryGetValue(topicName, out KafkaTopicOverride topicOverride);

        int numPartitions = topicOverride?.NumPartitions ?? defaults.NumPartitions;
        short replicationFactor = topicOverride?.ReplicationFactor ?? defaults.ReplicationFactor;
        int retentionDays = topicOverride?.RetentionDays ?? defaults.RetentionDays;
        string retentionMs = ((long)TimeSpan.FromDays(retentionDays).TotalMilliseconds).ToString();

        return new()
        {
            Name = topicName,
            NumPartitions = numPartitions,
            ReplicationFactor = replicationFactor,
            Configs = new() { ["retention.ms"] = retentionMs }
        };
    }
}