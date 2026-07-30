namespace Company.ProductService.Messaging;

public sealed record KafkaOptions
{
    public const string SectionName = "Kafka";

    public string BootstrapServers { get; init; } = "localhost:29092";
    public KafkaTopicDefaults Defaults { get; init; } = new();
    public Dictionary<string, KafkaTopicOverride> Topics { get; init; } = new();
}

public sealed record KafkaTopicDefaults
{
    public int NumPartitions { get; init; } = 1;
    public short ReplicationFactor { get; init; } = 1;
    public int RetentionDays { get; init; } = 7;
}

public sealed record KafkaTopicOverride
{
    public int? NumPartitions { get; init; }
    public short? ReplicationFactor { get; init; }
    public int? RetentionDays { get; init; }
}