namespace realTimeMessagingWebAppInfra.Storage.Services;

public interface IObjectStorageService
{
    Task<string> UploadObjectFromMemoryAsync(string bucket, string objectKey, string content, string contentType); // kind of assuming they will make it base64 string, not sure if thats a good idae
    Task<string> GetObjectFromBucketAsync(string bucket, string objectKey); // dont thing we meed this rn
    Task<string> DeleteObjectFromBucketAsync(string bucket, string objectKey);
    // dont think we need update for now
    Task<string> GetObjectUrlForClientAsync(string bucket, string objectKey);
    Task<string> CreateObjectUrlForClientUploadAsync(string bucket, string objectKey, string contentType);
}
