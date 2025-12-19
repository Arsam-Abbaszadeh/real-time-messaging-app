using realTimeMessagingWebApp.Enums;

namespace realTimeMessagingWebApp.Entities
{
    public class GroupChat
    {
        public Guid GroupChatId { get; set; }
        public string GroupChatName { get; set; }
        public DateTime CreationDate { get; set; }
        public Guid GroupChatCreatorId { get; set; }
        public Guid GroupChatAdminId { get; set; }
        public GroupChatType ChatType { get; set; }
        
        // nav properties
        public ICollection<User> GroupChatMembers { get; set; } = new HashSet<User>();

        public User GroupChatCreator; // add GC creator and GC admin as nav properties

        public User GroupChatAdmin; // add GC creator and GC admin as nav properties
    }
}
