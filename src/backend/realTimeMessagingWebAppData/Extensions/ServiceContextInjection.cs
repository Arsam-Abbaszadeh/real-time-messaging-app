using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace realTimeMessagingWebAppData.Extensions;

public static class ServiceContextInjection
{
    public static IServiceCollection AddRealtimeMessagingWebAppContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<Context>(options =>
            options.UseNpgsql(connectionString));

        return services;
    }

}
