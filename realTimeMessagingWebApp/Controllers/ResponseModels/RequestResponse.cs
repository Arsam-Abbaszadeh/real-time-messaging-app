namespace realTimeMessagingWebApp.Controllers.ResponseModels
{
    public class RequestResponse
    {
        // Can only think of the login usecase here, but im sure there are more
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string? AccessToken { get; set; }
        public RequestResponse(bool isSuccess, string message, string? accessToken = null)
        {
            IsSuccess = isSuccess;
            Message = message;
            AccessToken = accessToken;
        }
        public RequestResponse() : this(false, string.Empty) { } // Default constructor for serialization
    }
}
