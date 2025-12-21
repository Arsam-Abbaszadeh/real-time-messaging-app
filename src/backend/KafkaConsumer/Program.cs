using KafkaConsumer;
using realTimeMessagingWebAppData.Extensions;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddRealtimeMessagingWebAppContext(connectionString);

var host = builder.Build();
host.Run();
