
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using realTimeMessagingWebAppInfra.Configurations;
using realTimeMessagingWebAppInfra.Storage.Services;

namespace realTimeMessagingWebAppInfra.Storage.Extensions;

public static class ObjectStorageServiceExtension
{
    public static void RegisterObjectStorage(this IServiceCollection services, R2StorageOptions? options, bool includeDependencies = true)
    {
        services.AddSingleton<IObjectStorageService, ObjectStorageService>();

        if (includeDependencies)
        {
            ArgumentNullException.ThrowIfNull(options);

            services.AddSingleton<IAmazonS3>(_ =>
            {
                var config = new AmazonS3Config
                {
                    ServiceURL = options.ServiceUrl,
                    ForcePathStyle = true
                };
                
                var creds = new BasicAWSCredentials(options.AccessKeyId, options.SecretAccessKey);
                return new AmazonS3Client(creds, config);
            });
        }
    }
}
