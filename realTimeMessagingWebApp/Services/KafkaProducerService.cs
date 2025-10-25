
namespace realTimeMessagingWebApp.Services;

public class KafkaProducerService : IKafkaProducerService
{
    public Task ProduceAsync<TKey, TValue>(string topic, TKey key, TValue value)
    {
        throw new NotImplementedException();
    }
}
q