using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using realTimeMessagingWebApp.Configurations;
using realTimeMessagingWebApp.Hubs;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Services.ResponseModels;
using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebAppInfra.Persistence.Extensions;
using realTimeMessagingWebAppInfra.Persistence.Data.Repository;
using realTimeMessagingWebAppInfra.Storage.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>(optional: true);
}

var configs = builder.Configuration;

var connectionString = configs.GetConnectionString("DefaultConnection");
builder.Services.AddRealtimeMessagingWebAppContext(connectionString);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGroupChatService, GroupChatService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFriendShipRequestService, FriendShipRequestService>();
builder.Services.AddScoped<ICustomRepository<GroupChat>, GroupChatRepositry>();
builder.Services.AddScoped<IMessageSequenceTrackerService, MessageSequenceTrackerService>();
builder.Services.AddScoped<RelationShipService>();

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
builder.Services.RegisterObjectStorageServiceFromInfraSettings();

// might cleaner to add data annotations to the classes and use ValidateDataAnnotations() instead of individual validations
// probs cleaner to move to extension method
builder.Services
    .AddOptions<KafkaConfigurations>()
    .Bind(configs.GetSection(KafkaConfigurations.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Brokers), $"{KafkaConfigurations.SectionName}:Brokers is required.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Topic), $"{KafkaConfigurations.SectionName}:Topic is required.")
    .Validate(o => o.FlushTimeoutSeconds > 0, $"{KafkaConfigurations.SectionName}:FlushTimeoutSeconds must be > 0.")
    .ValidateOnStart();

builder.Services
    .AddOptions<JwtCreationOptions>()
    .Bind(configs.GetSection(JwtCreationOptions.SectionName))
    .Validate(o => o.AccessExpiration > 0, $"{JwtCreationOptions.SectionName}:AccessExpiration must be > 0")
    .Validate(o => o.RefreshExpiration > 0, $"{JwtCreationOptions.SectionName}:RefreshExpiration must be > 0.");

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(configs.GetSection(JwtOptions.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey), $"{JwtOptions.SectionName}:SecretKey is required.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Issuer), $"{JwtOptions.SectionName}:Issuer is required.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Audience), $"{JwtOptions.SectionName}:Audience is required.")
    .Validate(o => o.ClockSkewSeconds >= 0, $"{JwtOptions.SectionName}:ClockSkewSeconds must be >= 0.")
    .ValidateOnStart();

builder.Services.AddControllers();
// consider adding options later, like try reconnection or whatever
// Will probs need to add JSON serailzation options given we are using DTOs,
    // check out https://learn.microsoft.com/en-us/aspnet/core/signalr/configuration?view=aspnetcore-10.0&tabs=dotnet
builder.Services.AddSignalR(); 

const string chatHubPath = "/chathub";
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var jwtOptions = configs.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                  ?? throw new InvalidOperationException("JWT configuration section is missing.");

        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(jwtOptions.ClockSkewSeconds)
        };

        o.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var path = context.HttpContext.Request.Path;
                if (path.StartsWithSegments(chatHubPath))
                {
                    // Browser WebSockets/SSE commonly use this:
                    var accessToken = context.Request.Query["access_token"].ToString();
                    if (!string.IsNullOrWhiteSpace(accessToken))
                    {
                        context.Token = accessToken;
                        return Task.CompletedTask;
                    }

                    // Node clients may send Authorization header even for WebSockets:
                    var authHeader = context.Request.Headers.Authorization.ToString();
                    const string bearerPrefix = "Bearer ";
                    if (!string.IsNullOrWhiteSpace(authHeader) && authHeader.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        context.Token = authHeader[bearerPrefix.Length..].Trim();
                    }
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>(chatHubPath);
app.MapControllers();
//app.UseHttpsRedirection(); // dont need for now

app.Run();
