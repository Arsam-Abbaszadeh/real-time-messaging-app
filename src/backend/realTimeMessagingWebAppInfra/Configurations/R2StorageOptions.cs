namespace realTimeMessagingWebAppInfra.Configurations;

public sealed class R2StorageOptions
{
    public const string SectionName = "R2";

    public string AccountId { get; set; } = string.Empty;
    public string AccessKeyId { get; set; } = string.Empty;
    public string SecretAccessKey { get; set; } = string.Empty;
    public string BucketName { get; set; } = string.Empty;
    public string PublicBaseUrl { get; set; } = string.Empty; // optional (CDN / public endpoint)
    public string Region { get; set; } = "auto"; // R2 commonly uses "auto" for S3-compatible clients
}
