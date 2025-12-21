using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using realTimeMessagingWebAppData.Entities;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Services.ResponseModels;
using System.Text;
using realTimeMessagingWebAppData.Repository;
using realTimeMessagingWebAppData;
using realTimeMessagingWebAppData.Extensions;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddRealtimeMessagingWebAppContext(connectionString);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IGroupChatService, GroupChatService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFriendShipRequestService, FriendShipRequestService>();
builder.Services.AddScoped<ICustomRepository<GroupChat>, GroupChatRepositry>();
builder.Services.AddScoped<RelationShipService>();
builder.Services.AddScoped<IMessageSequenceTrackerService, MessageSequenceTrackerService>(); // could I guess be singleton

builder.Services.AddControllers();
builder.Services.AddSignalR();

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

app.MapControllers();

app.Run();
