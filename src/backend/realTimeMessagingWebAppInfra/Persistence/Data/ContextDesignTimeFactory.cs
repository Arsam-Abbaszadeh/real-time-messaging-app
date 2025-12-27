using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace realTimeMessagingWebAppInfra.Persistence.Data;

sealed class ContextDesignTimeFactory : IDesignTimeDbContextFactory<Context>
{
    public Context CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddUserSecrets<ContextDesignTimeFactory>(optional: true)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");


        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "No connection string found. Set ConnectionStrings:DefaultConnection via User Secrets for realTimeMessagingWebAppInfra.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<Context>()
            .UseNpgsql(connectionString);

        return new Context(optionsBuilder.Options);
    }
}