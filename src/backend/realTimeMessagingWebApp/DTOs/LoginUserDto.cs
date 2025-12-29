using System.ComponentModel.DataAnnotations;

namespace realTimeMessagingWebApp.DTOs
{
    public record LoginUserDto
    {
        [Required]
        public string UserName { get; init; }
        [Required]
        public string Password { get; init; } // NOT password hash

    }
}