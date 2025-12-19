namespace realTimeMessagingWebApp.DTOs
{
    public class GroupChatSummaryDto
    {
        public string GroupChatName { get; set; } = string.Empty;
        public DateTime GroupChatCreationDate { get; set; }

        public Guid GroupChatId { get; set; }
    }
}
