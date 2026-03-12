namespace realTimeMessagingWebApp.DTOs;

public record AttachementDto
{
    public Guid AttachementId { get; init; }
    public Guid MessageId { get; init; }
    public string AttachmentUrl { get; init; } = string.Empty;
    public string AttachmentMimeType { get; init; } = string.Empty;
}
