namespace realTimeMessagingWebApp.DTOs
{
    public record GroupChatSummaryDto
    {
        public string GroupChatName { get; init; } = string.Empty;
        public DateTime GroupChatCreationDate { get; init; }

        public Guid GroupChatId { get; init; }
    }
}
