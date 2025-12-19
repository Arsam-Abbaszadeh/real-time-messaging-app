namespace realTimeMessagingWebApp.Entities
{
    public class GroupChatConnector
    {
        // this table is for connecting user to gropu chats to have many to many relationship between users and groupchats
        public Guid GroupChatConnectorId { get; set; }
        public Guid GroupChatId { get; set; }
        public Guid UserId { get; set; }
        public DateTime JoinDate { get; set; }

        // Nav properties
        public GroupChat GroupChat { get; set; }
        public User User { get; set; }
    }
}
