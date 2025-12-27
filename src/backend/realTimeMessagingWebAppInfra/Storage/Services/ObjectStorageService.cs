using Amazon.S3;

namespace realTimeMessagingWebAppInfra.Storage.Services;

public class ObjectStorageService(IAmazonS3 s3Client) : IObjectStorageService
{
    readonly IAmazonS3 _s3Client = s3Client;

    public Task<string> CreateObjectUrlForClientUploadAsync(string bucket, string objectKey, string contentType)
    {
        throw new NotImplementedException();
    }

    public Task<string> DeleteObjectFromBucketAsync(string bucket, string objectKey)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetObjectFromBucketAsync(string bucket, string objectKey)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetObjectUrlForClientAsync(string bucket, string objectKey)
    {
        throw new NotImplementedException();
    }

    public Task<string> UploadObjectFromMemoryAsync(string bucket, string objectKey, string content, string contentType)
    {
        throw new NotImplementedException();
    }
}
