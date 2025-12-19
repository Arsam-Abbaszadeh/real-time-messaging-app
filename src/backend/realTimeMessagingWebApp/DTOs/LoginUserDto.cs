using System.ComponentModel.DataAnnotations;

namespace realTimeMessagingWebApp.DTOs
{
    public class LoginUserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; } // NOT password hash

    }
}