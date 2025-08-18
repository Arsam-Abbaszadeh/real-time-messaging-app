using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebApp.Data;
using realTimeMessagingWebApp.Services;

var builder = WebApplication.CreateBuilder(args);
// Connect db for context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<Context>(options => options.UseNpgsql(connectionString));

// Add services to the container
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddControllers();



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

app.UseAuthorization(); // havent implemented

app.MapControllers();

app.Run();
