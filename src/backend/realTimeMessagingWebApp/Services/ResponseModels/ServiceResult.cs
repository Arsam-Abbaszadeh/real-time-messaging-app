namespace realTimeMessagingWebApp.Services.ResponseModels
{
    public class ServiceResult
    {
        public bool IsSuccess { get; set; }
     
        public string Message { get; set; }

        public object? Data { get; set; } // optional data field
    }
}
