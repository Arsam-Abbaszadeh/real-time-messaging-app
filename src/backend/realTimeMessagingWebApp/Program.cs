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

var builder = WebApplication.CreateBuilder(args);
var configs = builder.Configuration;

var connectionString = configs.GetConnectionString("DefaultConnection");
builder.Services.AddRealtimeMessagingWebAppContext(connectionString);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGroupChatService, GroupChatService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFriendShipRequestService, FriendShipRequestService>();
builder.Services.AddScoped<ICustomRepository<GroupChat>, GroupChatRepositry>();
builder.Services.AddScoped<IMessageSequenceTrackerService, MessageSequenceTrackerService>(); // could I guess be singleton
builder.Services.AddScoped<RelationShipService>();

builder.Services.AddSingleton<IKafkaProducerService, KafkaProducerService>();

// consider configuring JWTs in and all other settings in general and then moving them to another method for a general add all configs extension
builder.Services.Configure<KafkaConfigurations>(configs.GetSection(nameof(KafkaConfigurations)));

builder.Services.AddControllers();
builder.Services.AddSignalR(); // consider adding options later, like try reconnection or whatever

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false; // idk what htis does
        o.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(45) // small buffer
        };

        // auth validation for singalR, ONLY USE FOR SIGNALR, as JWT cant be sent in header for websockets (usually)
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
