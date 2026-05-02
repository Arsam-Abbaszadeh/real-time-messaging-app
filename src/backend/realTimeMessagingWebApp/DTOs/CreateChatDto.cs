using System.ComponentModel.DataAnnotations;
using realTimeMessagingWebAppInfra.Persistence.Enums;

namespace realTimeMessagingWebApp.DTOs
{
    public record CreateChatDto
    {
        [Required]
        public string ChatName { get; init; }

        public ICollection<Guid> ChatMembers { get; init; } = new HashSet<Guid>();

        public ChatType ChatType { get; init; }
        
        public Guid? AdminId { get; init; }

    }
}
