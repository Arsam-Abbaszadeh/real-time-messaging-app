namespace realTimeMessagingWebApp.DTOs
{
    public class UserSummaryDto
    {
        public string UserName { get; set; }
        public DateTime SignUpDate { get; set; }

        //public int PasswordLength { get; set; } // experimental feature for user profile management, if you wanna add change password feature
    }
}
