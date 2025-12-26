namespace KafkaConsumer;

public class KafkaConsumerService(ILogger<KafkaConsumerService> logger) : BackgroundService
{
    readonly ILogger<KafkaConsumerService> _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("KafkaConsumer running at: {time}", DateTimeOffset.Now); // check if this aint already being done
        }

        while (!stoppingToken.IsCancellationRequested) // any other conditions to stop?
        {
            // implement stuff to receive messages.
            // once messages can be recieved by the producer, then implement logic to deal with them here.
                // implement DB services to save messages and stuff.
        }
    }
}
