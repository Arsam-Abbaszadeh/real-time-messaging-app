namespace realTimeMessagingWebApp.DTOs
{
    public record UserSummaryDto
    {
        public string UserName { get; init; }
        public DateTime SignUpDate { get; init; }

        //public int PasswordLength { get; set; } // experimental feature for user profile management, if you wanna add change password feature
    }
}
