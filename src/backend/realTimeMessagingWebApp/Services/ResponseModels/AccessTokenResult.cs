namespace realTimeMessagingWebApp.Services.ResponseModels
{
    public class AccessTokenResult
    {
        public string? AccessToken { get; set; }

        public bool ValidRefreshToken { get; set; }

        public string Message { get; set; }

    }
}
