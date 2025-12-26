
using System.Text.Json;
using Confluent.Kafka;
using realTimeMessagingWebApp.Configurations;
using realTimeMessagingWebApp.DTOs;

namespace realTimeMessagingWebApp.Services;

public class KafkaProducerService : IKafkaProducerService, IAsyncDisposable
{
    readonly IProducer<string, string> _producer; // should be a singleton
    readonly KafkaConfigurations _kafkaConfigurations;
    readonly int _flushTimeoutSeconds;

    public KafkaProducerService(KafkaConfigurations kafkaConfigurations) // consider adding logger
    {
        _kafkaConfigurations = kafkaConfigurations;
        _flushTimeoutSeconds = kafkaConfigurations.FlushTimeoutSeconds;
        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaConfigurations.Brokers,
            EnableIdempotence = _kafkaConfigurations.EnableIdempotence,
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task ProduceAsync(string topic, string key, UserChatMessageRecieveDto value)
    {
        var payload = JsonSerializer.Serialize(value);
        var message = new Message<string, string>
        {
            Key = key,
            Value = payload
        };

        await _producer.ProduceAsync(topic, message);
    }

    public ValueTask DisposeAsync()
    {
        // Try to deliver any queued messages before closing.
        _producer.Flush(TimeSpan.FromSeconds(_flushTimeoutSeconds));
        _producer.Dispose();
        return ValueTask.CompletedTask;
    }

}
