using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using realTimeMessagingWebAppInfra.Configurations;
using realTimeMessagingWebAppInfra.Storage.Services;

namespace realTimeMessagingWebAppInfra.Storage.Extensions;

public static class ObjectStorageServiceExtension
{
    private const string InfraSettingsFileName = "librarySettings.json";
    public static void RegisterObjectStorageServiceFromInfraSettings(this IServiceCollection services)
    {
        var currentAssembly = typeof(ObjectStorageServiceExtension).Assembly;
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile(InfraSettingsFileName, optional: false, reloadOnChange: true)
            .AddUserSecrets(currentAssembly, optional: false)
            .Build();

        services.AddOptions<R2AccessOptions>()
            .Bind(configBuilder.GetSection(R2AccessOptions.SectionName))
            .ValidateOnStart(); // dont think we added any validations but, maybe just add type annotations

        services.AddOptions<R2BucketOptions>()
            .Bind(configBuilder.GetSection(R2BucketOptions.SectionName));

        services.AddSingleton<IAmazonS3>(sp =>
        {
            var accessOptions = sp.GetRequiredService<IOptions<R2AccessOptions>>().Value;
            ArgumentNullException.ThrowIfNull(accessOptions);

            var config = new AmazonS3Config // TODO check for other important configs
            {
                ServiceURL = accessOptions.ServiceUrl,
                ForcePathStyle = true,
            };
        
            var creds = new BasicAWSCredentials(accessOptions.AccessKeyId, accessOptions.SecretAccessKey);
            return new AmazonS3Client(creds, config);
        });
        services.AddSingleton<IObjectStorageService, ObjectStorageService>();
    }
}
