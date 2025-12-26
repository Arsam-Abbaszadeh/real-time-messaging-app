using System.ComponentModel.DataAnnotations;
using realTimeMessagingWebAppInfra.Persistence.Enums;

namespace realTimeMessagingWebApp.DTOs
{
    public class CreateGroupChatDto
    {
        [Required]
        public string GroupChatName { get; set; }

        public ICollection<Guid> GroupChatMembers { get; set; } = new HashSet<Guid>();

        public GroupChatType GroupChatType { get; set; } // I dont like how this needs to be synced across

        public Guid? Admin { get; set; }

        //public Guid AdminId { get; set; } // We can get this from the JWT

    }
}
