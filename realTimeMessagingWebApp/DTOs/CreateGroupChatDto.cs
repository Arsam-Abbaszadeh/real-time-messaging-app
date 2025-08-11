using System.ComponentModel.DataAnnotations;
using realTimeMessagingWebApp.Entities;

namespace realTimeMessagingWebApp.DTOs
{
    public class CreateGroupChatDto
    {
        [Required]
        public string GroupChatName { get; set; }

        ICollection<User> GroupChatMembers { get; set; } = new HashSet<User>();

        public Guid AdminId { get; set; }

    }
}
