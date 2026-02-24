namespace realTimeMessagingWebApp.Services.ResponseModels
{
    public class ChatAuthResult
    {
        public bool IsAdmin { get; set; }
        public bool IsMember { get; set; }
        public bool IsSelfAction { get; set; }

        public bool CanRemoveMember => IsAdmin || IsSelfAction;
    }
}
