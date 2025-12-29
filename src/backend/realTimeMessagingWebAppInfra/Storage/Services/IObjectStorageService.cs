using realTimeMessagingWebAppInfra.Storage.Constants;
using realTimeMessagingWebAppInfra.Storage.Services.ResponseModel;

namespace realTimeMessagingWebAppInfra.Storage.Services;

public interface IObjectStorageService
{
    Task<ObjectResponseEtag> UploadObjectFromMemoryAsync(string bucket, string objectKey, string content, string contentType); // kind of assuming they will make it base64 string, not sure if thats a good idae
    Task<ObjectResponseEtag> GetObjectFromBucketAsync(string bucket, string objectKey); // dont thing we meed this rn
    Task<ObjectResponse> DeleteObjectFromBucketAsync(string bucket, string objectKey);
    // dont think we need update for now
    Task<string> GetObjectUrlForClientRenderingAsync(BucketKeys bucketKey, string objectKey);
    Task<string> CreateObjectUrlForClientUploadAsync(BucketKeys bucketKey, string objectKey, string contentType, DateTime uploadedAtUtc);
}
