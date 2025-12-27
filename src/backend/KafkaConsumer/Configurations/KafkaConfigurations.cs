namespace KafkaConsumer.Configurations;


#nullable disable
public class KafkaConfigurations
{
    public const string SectionName = "KafkaConfigurations";
    public string Brokers { get; set; }
    public string ConsumerGroup { get; set; }
}

