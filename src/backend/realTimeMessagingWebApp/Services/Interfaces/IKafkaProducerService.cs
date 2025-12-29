using realTimeMessagingWebApp.DTOs;

namespace realTimeMessagingWebApp.Services;

public interface IKafkaProducerService
{
    Task ProduceAsync(string topic, string key, UserChatMessageRecieveDto value);
}