namespace realTimeMessagingWebApp.DTOs
{
    public class UserAccessResultDto // might need to add user ID
    {
        public bool IsSuccessful { get; set; }

        public string Message { get; set; }
        public string? AccessToken { get; set; } = null;
    }
}
