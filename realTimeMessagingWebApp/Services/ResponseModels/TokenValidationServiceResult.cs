namespace realTimeMessagingWebApp.Services.ResponseModels
{
    public class TokenValidationServiceResult
    {
        public bool validationResult;

        public bool? validationSuccess;

        public string message;

        // will need to add claims property later
    }
}
