using System.ComponentModel.DataAnnotations;
using realTimeMessagingWebAppInfra.Persistence.Enums;

namespace realTimeMessagingWebApp.DTOs
{
    public record CreateGroupChatDto
    {
        [Required]
        public string GroupChatName { get; init; }

        public ICollection<Guid> GroupChatMembers { get; init; } = new HashSet<Guid>();

        public GroupChatType GroupChatType { get; init; } // I dont like how this needs to be synced across

        public Guid? Admin { get; init; }

        //public Guid AdminId { get; set; } // We can get this from the JWT

    }
}
