using System.Globalization;
using System.Net;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using realTimeMessagingWebAppInfra.Configurations;
using realTimeMessagingWebAppInfra.Storage.Constants;
using realTimeMessagingWebAppInfra.Storage.Services.ResponseModel;

namespace realTimeMessagingWebAppInfra.Storage.Services;

public class ObjectStorageService(
    IAmazonS3 s3Client,
    ILogger<ObjectStorageService> logger,
    IOptionsMonitor<R2BucketOptions> bucketOptions
) : IObjectStorageService
{
    readonly IAmazonS3 _s3Client = s3Client;
    readonly ILogger<ObjectStorageService> _logger = logger;
    readonly R2BucketOptions _bucketOptions = bucketOptions.CurrentValue;

    public async Task<ObjectResponse> DeleteObjectFromBucketAsync(string bucket, string objectKey)
    {
        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = bucket,
                Key = objectKey,
            };

            var response = await _s3Client.DeleteObjectAsync(deleteRequest);
            if (response.HttpStatusCode is >= HttpStatusCode.OK and <= HttpStatusCode.NoContent)
            {
                _logger.LogInformation(
                    "Successfully deleted object from bucket {Bucket} with key {ObjectKey}. Status code: {StatusCode}",
                    bucket, objectKey, response.HttpStatusCode);

                return new()
                {
                    IsSuccess = true,
                    StatusCode = response.HttpStatusCode
                };
            }

            _logger.LogError(
                "Failed to delete object from bucket {Bucket} with key {ObjectKey}. Status code: {StatusCode}",
                bucket, objectKey, response.HttpStatusCode);

            return new()
            {
                IsSuccess = false,
                StatusCode = response.HttpStatusCode,
                ErrorMessage = $"Failed to delete object. Status code: {response.HttpStatusCode}"
            };
        }
        catch (AmazonS3Exception ex)
        {
            _logger.LogError(
                ex,
                "{ExceptionType} when deleting object from bucket {Bucket} with key {ObjectKey}. Error Message: {ErrorMessage}",
                ex.GetType().FullName, bucket, objectKey, ex.Message);

            return new()
            {
                IsSuccess = false,
                StatusCode = ex.StatusCode,
                ErrorMessage = $"{ex.GetType().FullName} when deleting object from bucket {bucket} with key {objectKey}, Error Message: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unknown exception of type {ExceptionType} when deleting object from bucket {Bucket} with key {ObjectKey}. Error Message: {ErrorMessage}",
                ex.GetType().FullName, bucket, objectKey, ex.Message);

            return new()
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessage = $"Unknown exception when deleting object from bucket {bucket} with key {objectKey}, Error Message: {ex.Message}"
            };
        }
    }
    public async Task<ObjectResponseEtag> UploadObjectFromMemoryAsync(string bucket, string objectKey, string content, string contentType)
    {
        var contentBytes = Encoding.UTF8.GetBytes(content);
        // TransferUtility is better for larger objects but I wanted the response object to get the etag and request code directly
        using var memoryStream = new MemoryStream(contentBytes);
        var putRequest = new PutObjectRequest
        {
            BucketName = bucket,
            Key = objectKey,
            ContentType = contentType,
            InputStream = memoryStream,
            DisablePayloadSigning = true,
            DisableDefaultChecksumValidation = true
        };
        var response = await _s3Client.PutObjectAsync(putRequest);

        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            _logger.LogInformation("Succesfully uploaded object to bucket {Bucket} with key {ObjectKey}. Status code: {StatusCode}", bucket, objectKey, response.HttpStatusCode);
            return new()
            {
                ETag = response.ETag,
                IsSuccess = true,
                statusCode = response.HttpStatusCode
            };
        }
        else
        {
            _logger.LogError("Failed to upload object to bucket {Bucket} with key {ObjectKey}. Status code: {StatusCode}", bucket, objectKey, response.HttpStatusCode);
            return new()
            {
                IsSuccess = false,
                statusCode = response.HttpStatusCode,
                ErrorMessage = $"Failed to upload object. Status code: {response.HttpStatusCode}"
            };
        }
    }

    public async Task<ObjectResponseEtag> GetObjectFromBucketAsync(string bucket, string objectKey)
    {
        var getRequest= new GetObjectRequest
        {
            BucketName = bucket,
            Key = objectKey
        };

        var response = await _s3Client.GetObjectAsync(getRequest);
        if (response.HttpStatusCode == HttpStatusCode.OK)
        {
            using var reader = new StreamReader(response.ResponseStream);
            var content = await reader.ReadToEndAsync();
            _logger.LogInformation("Successfully retrieved object from bucket {Bucket} with key {ObjectKey}. Status code: {StatusCode}", bucket, objectKey, response.HttpStatusCode);
            return new()
            {
                ETag = response.ETag,
                IsSuccess = true,
                statusCode = response.HttpStatusCode
            };

        }
        else
        {
            _logger.LogError("Failed to retrieve object from bucket {Bucket} with key {ObjectKey}. Status code: {StatusCode}", bucket, objectKey, response.HttpStatusCode);
            return new()
            {
                IsSuccess = false,
                statusCode = response.HttpStatusCode,
                ErrorMessage = $"Failed to retrieve object. Status code: {response.HttpStatusCode}"
            };
        }
    }

    public async Task<string> CreateObjectUrlForClientUploadAsync(BucketKeys bucketKey, string objectKey, string contentType, DateTime uploadedAtUtc)
    {
        var metaData = new MetadataCollection();
        // round trip the datetime to ensure we dont lose precision
        metaData.Add("UploadedAtFromBackend", uploadedAtUtc.ToString("O", CultureInfo.InvariantCulture));

        var presignedUrlRequest = new GetPreSignedUrlRequest
        {
            BucketName = BucketMappings.GetBucketNameFromKey(bucketKey),
            Key = objectKey,
            Verb = HttpVerb.PUT,
            Expires = DateTime.UtcNow.AddHours(_bucketOptions.BucketUploadExpirationInMinutes),
            ContentType = contentType
        };
        var presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedUrlRequest);
        return presignedUrl;
    }

    public async Task<string> GetObjectUrlForClientRenderingAsync(BucketKeys bucketKey, string objectKey)
    {
        if (bucketKey == BucketKeys.Public)
        {
            return $"{_bucketOptions.PublicBucketUrl}/{objectKey}";
        }
        else // assumes only other bucket is private
        {
            var presignedUrlRequest = new GetPreSignedUrlRequest
            {
                BucketName = BucketMappings.GetBucketNameFromKey(bucketKey),
                Key = objectKey,
                Verb = HttpVerb.GET, // this is for rendering so pre sure its GET, TODO verify assumption
                Expires = DateTime.UtcNow.AddMinutes(_bucketOptions.PrivateBucketDownloadExpirationInHours)
            };

            // async version isnt really required but keeping consistent with other methods
            var presignedUrl = await _s3Client.GetPreSignedURLAsync(presignedUrlRequest);
            return presignedUrl;
        }
    }
}
