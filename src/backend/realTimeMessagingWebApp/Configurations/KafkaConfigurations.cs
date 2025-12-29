namespace realTimeMessagingWebApp.Configurations;

public sealed class KafkaConfigurations
{
    public const string SectionName = "KafkaConfigurations";

    public required string Brokers { get; init; }
    public required string Topic { get; init; }
    public string Key { get; init; }

    public bool EnableIdempotence { get; init; }
    public int FlushTimeoutSeconds { get; init; }
}
