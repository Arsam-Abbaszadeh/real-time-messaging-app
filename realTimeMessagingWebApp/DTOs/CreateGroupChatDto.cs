using System.ComponentModel.DataAnnotations;
using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Enums;

namespace realTimeMessagingWebApp.DTOs
{
    public class CreateGroupChatDto
    {
        [Required]
        public string GroupChatName { get; set; }

        public ICollection<User> GroupChatMembers { get; set; } = new HashSet<User>();

        public GroupChatType GroupChatType { get; set; } // I dont like how this needs to be synced across

        public Guid? Admin { get; set; }

        //public Guid AdminId { get; set; } // We can get this from the JWT

    }
}
