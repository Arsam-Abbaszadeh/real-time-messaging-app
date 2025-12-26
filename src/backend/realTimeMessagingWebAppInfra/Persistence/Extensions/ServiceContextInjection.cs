using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using realTimeMessagingWebAppInfra.Persistence.Data;

namespace realTimeMessagingWebAppInfra.Persistence.Extensions;

public static class ServiceContextInjection
{
    public static IServiceCollection AddRealtimeMessagingWebAppContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<Context>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }
}
