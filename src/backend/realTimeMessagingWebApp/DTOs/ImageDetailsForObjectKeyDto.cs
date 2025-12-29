namespace realTimeMessagingWebApp.DTOs;

# nullable disable
public record ImageDetailsForObjectKeyDto
{
    public Guid UserId { get; init; }
    public Guid ChatId { get; init; }
    public string FileExtension { get; init; }
    public string FileType { get; init; }
    public DateTime UploadedAt { get; init; }
}
