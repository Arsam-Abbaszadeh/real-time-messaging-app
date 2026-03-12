namespace realTimeMessagingWebApp.DTOs;

public record MessageDto
{
    public Guid ChatId { get; init; }
    public Guid MessageSenderId { get; init; }
    public Guid MessageId { get; init; }
    public bool MessageIsEdited { get; init; }
    public DateTime MessageSentAt { get; init; }
    public uint MessageSequenceNumber { get; init; }
    public string MessageContent { get; init; } = string.Empty;
    public ICollection<AttachementDto> MessageAttachments { get; init; } = [];
}