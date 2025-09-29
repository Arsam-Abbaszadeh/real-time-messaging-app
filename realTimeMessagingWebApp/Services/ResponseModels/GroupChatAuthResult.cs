namespace realTimeMessagingWebApp.Services.ResponseModels
{
    public class GroupChatAuthResult
    {
        public bool IsAdmin { get; set; }
        public bool IsMember { get; set; }
        public bool IsSelfAction { get; set; }

        public bool CanRemoveMember => IsAdmin || IsSelfAction;
    }
}
