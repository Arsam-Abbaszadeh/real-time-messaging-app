using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebApp.DTOs;

public class UserChatMessageRecieveDto
{
    public Guid GroupChatId { get; set; }
    public Guid UserId { get; set; }
    public string MessageContent { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public ICollection<MessageAttachment> Attachments { get; set; } = new HashSet<MessageAttachment>();
}
