using realTimeMessagingWebAppInfra.Persistence.Enums;

namespace realTimeMessagingWebAppInfra.Persistence.Entities;

public class Chat
{
    public Guid ChatId { get; set; }
    public string ChatName { get; set; }
    public string ChatImageUrl { get; set; }
    public DateTime CreationDate { get; set; }
    public Guid ChatCreatorId { get; set; }
    public Guid ChatAdminId { get; set; }
    public ChatType ChatKind { get; set; }
    
    // nav properties
    public ICollection<User> ChatMembers { get; set; } = new HashSet<User>();

    public User ChatCreator; // add GC creator and GC admin as nav properties

    public User ChatAdmin; // add GC creator and GC admin as nav properties
}
