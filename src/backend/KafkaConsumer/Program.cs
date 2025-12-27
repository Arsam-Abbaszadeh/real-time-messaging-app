using KafkaConsumer;
using KafkaConsumer.Configurations;
using realTimeMessagingWebAppInfra.Persistence.Extensions;

var builder = Host.CreateApplicationBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

builder.Services.AddHostedService<KafkaConsumerService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddRealtimeMessagingWebAppContext(connectionString);

builder.Services
    .AddOptions<KafkaConfigurations>()
    .Bind(builder.Configuration.GetSection(KafkaConfigurations.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.ConsumerGroup), $"{KafkaConfigurations.SectionName}:ConsumerGroup is required.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Brokers), $"{KafkaConfigurations.SectionName}:Brokers is required.")
    .ValidateOnStart();

var host = builder.Build();
host.Run();
