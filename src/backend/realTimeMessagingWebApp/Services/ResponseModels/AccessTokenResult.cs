namespace realTimeMessagingWebApp.Services.ResponseModels;

public class AccessTokenResult
{
    public string? AccessToken { get; set; }
    public DateTime? AccessTokenExpiration { get; init; } = null;

    public bool ValidRefreshToken { get; set; }

    public string Message { get; set; }

}
