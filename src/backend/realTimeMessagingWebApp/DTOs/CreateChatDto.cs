using System.ComponentModel.DataAnnotations;
using realTimeMessagingWebAppInfra.Persistence.Enums;

namespace realTimeMessagingWebApp.DTOs
{
    public record CreateChatDto
    {
        [Required]
        public string ChatName { get; init; }

        public ICollection<Guid> ChatMembers { get; init; } = new HashSet<Guid>();

        public ChatType ChatType { get; init; } // I dont like how this needs to be synced across

        public Guid? Admin { get; init; }

        //public Guid AdminId { get; set; } // We can get this from the JWT

    }
}
