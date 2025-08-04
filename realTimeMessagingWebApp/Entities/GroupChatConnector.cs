namespace realTimeMessagingWebApp.Entities
{
    public class GroupChatConnector
    {
        // this table is for connecting user to gropu chats to have many to many relationship between users and groupchats
        public Guid GroupChatConnectorID { get; set; }
        public Guid GroupChatID { get; set; }
        public Guid UserID { get; set; }
        public DateTime JoinDate { get; set; }

        // Nav properties
        public GroupChat GroupChat { get; set; }
        public User User { get; set; }
    }
}
