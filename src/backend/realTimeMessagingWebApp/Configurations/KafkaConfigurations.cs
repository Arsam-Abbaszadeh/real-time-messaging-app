namespace realTimeMessagingWebApp.Configurations;

public class KafkaConfigurations
{
    public string Brokers { get; set; }
    public string Topic { get; set; }
    public string Key { get; set; }
    public bool EnableIdempotence { get; set; }
    public int FlushTimeoutSeconds { get; set; }
}
