namespace realTimeMessagingWebAppInfra.Persistence.Entities;

public class ChatConnector
{
    // this table is for connecting user to chats to have many to many relationship between users and chats
    public Guid ChatConnectorId { get; set; }
    public Guid ChatId { get; set; }
    public Guid UserId { get; set; }
    public DateTime JoinDate { get; set; }

    // Nav properties
    public Chat Chat { get; set; }
    public User User { get; set; }
}
