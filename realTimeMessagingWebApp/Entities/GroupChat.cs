using realTimeMessagingWebApp.Enums;

namespace realTimeMessagingWebApp.Entities
{
    public class GroupChat
    {
        public Guid GroupChatId { get; set; }
        public string GroupChatName { get; set; }
        public DateTime creationDate { get; set; }
        public Guid GroupChatCreator { get; set; }
        public Guid? GroupChatAdmin { get; set; }
        public GroupChatType ChatType { get; set; }
        
        // nav properties
        public ICollection<User> GroupChatMembers { get; set; } = new HashSet<User>();

    }
}
