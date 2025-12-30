namespace realTimeMessagingWebApp.Configurations;

public sealed class KafkaConfigurations : IConfigModel
{
    public static string SectionName => "KafkaConfigurations";

    public required string Brokers { get; init; }
    public required string Topic { get; init; }
    public required string Key { get; init; }
    public required bool EnableIdempotence { get; init; }
    public required int FlushTimeoutSeconds { get; init; }
}
