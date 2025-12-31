namespace realTimeMessagingWebApp.DTOs;

public record UserChatMessageRecieveDto
{
    public Guid ChatId { get; init; }
    public Guid UserId { get; init; }
    public string MessageContent { get; init; } = string.Empty;
    public DateTime SentAt { get; init; }
    public ICollection<ImageDetailsCloudDto> Objectkeys { get; init; } = [];
}
