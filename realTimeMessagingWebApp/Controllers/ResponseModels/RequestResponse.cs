namespace realTimeMessagingWebApp.Controllers.ResponseModels
{
    public class RequestResponse
    {
        // Can only think of the login usecase here, but im sure there are more
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public RequestResponse(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
        public RequestResponse() : this(false, string.Empty) { } // Default constructor for serialization
    }
}
