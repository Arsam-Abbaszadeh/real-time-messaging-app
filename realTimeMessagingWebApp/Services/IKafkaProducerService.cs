namespace realTimeMessagingWebApp.Services;

public interface IKafkaProducerService
{
    Task ProduceAsync<TKey, TValue>(string topic, TKey key, TValue value);
}