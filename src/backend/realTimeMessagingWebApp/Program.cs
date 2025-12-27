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
using realTimeMessagingWebAppInfra.Configurations;

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

// might cleaner to add data annotations to the classes and use ValidateDataAnnotations() instead of individual validations
builder.Services
    .AddOptions<KafkaConfigurations>()
    .Bind(configs.GetSection(KafkaConfigurations.SectionName))
    .Validate(o => !string.IsNullOrWhiteSpace(o.Brokers), $"{KafkaConfigurations.SectionName}:Brokers is required.")
    .Validate(o => !string.IsNullOrWhiteSpace(o.Topic), $"{KafkaConfigurations.SectionName}:Topic is required.")
    .Validate(o => o.FlushTimeoutSeconds > 0, $"{KafkaConfigurations.SectionName}:FlushTimeoutSeconds must be > 0.")
    .ValidateOnStart();

builder.Services
    .AddOptions<JwtCreationOptions>()
    .Bind(configs.GetSection(JwtOptions.SectionName))
    .Validate(o => o.AccessExpiration > 0, $"{JwtOptions.SectionName}:AccessExpiration must be > 0")
    .Validate(o => o.RefreshExpiration > 0, $"{JwtOptions.SectionName}:RefreshExpiration must be > 0.");

// cant be botherd with validation on this one
builder.Services
    .AddOptions<R2StorageOptions>()
    .Bind(configs.GetSection(JwtOptions.SectionName));

var r2Options = builder.Configuration.GetSection(R2StorageOptions.SectionName).Get<R2StorageOptions>()
          ?? throw new InvalidOperationException("R2 Options section is missing.");
builder.Services.RegisterObjectStorage(r2Options);

builder.Services.AddControllers();
builder.Services.AddSignalR(); // consider adding options later, like try reconnection or whatever

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
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
                var accessToken = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/Chat"))
                {
                    context.Token = accessToken;
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

//app.UseHttpsRedirection(); // dont need for now

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/Chat"); // not fully sure how this connection works yet
app.MapControllers();

app.Run();
