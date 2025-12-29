
namespace realTimeMessagingWebAppInfra.Configurations;

#nullable disable
public sealed class R2AccessOptions
{
    public const string SectionName = "R2";

    public string AccountId { get; set; }
    public string AccessKeyId { get; set; }
    public string SecretAccessKey { get; set; }
    public string ServiceUrl => $"https://{AccountId}.r2.cloudflarestorage.com";

    // Dont need em yet pre sure, TODO check if they are useful
    //public string PublicBaseUrl { get; set; }// optional (CDN / public endpoint)
    //public string Region { get; set; } = "auto"; // R2 commonly uses "auto" for S3-compatible clients
}
