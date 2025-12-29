namespace realTimeMessagingWebApp.DTOs
{
    public record UserAccessResultDto // might need to add user ID
    {
        public bool IsSuccessful { get; init; }

        public string Message { get; init; }
        public string? AccessToken { get; init; } = null;
    }
}
