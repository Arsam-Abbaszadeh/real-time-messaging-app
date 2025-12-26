using KafkaConsumer;
using realTimeMessagingWebAppInfra.Persistence.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<KafkaConsumerService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddRealtimeMessagingWebAppContext(connectionString);

var host = builder.Build();
host.Run();
