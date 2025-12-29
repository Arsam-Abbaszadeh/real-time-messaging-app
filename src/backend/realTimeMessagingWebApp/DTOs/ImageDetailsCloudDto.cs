namespace realTimeMessagingWebApp.DTOs;

public record ImageDetailsCloudDto
{
    public string objectKey {  get; init; }
    public string objectFileExtension { get; init; }
    public string objectMimType { get; init; }
}
