namespace realTimeMessagingWebApp.Controllers.ResponseModels;

public class RequestResponse(bool isSuccess, string message, string? accessToken = null)
{
    // Can only think of the login usecase here, but im sure there are more
    public bool IsSuccess { get; set; } = isSuccess;
    public string Message { get; set; } = message;
    public string? AccessToken { get; set; } = accessToken;

    public RequestResponse() : this(false, string.Empty) { } // Default constructor for serialization
}
