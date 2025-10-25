namespace realTimeMessagingWebApp.Services;

public interface IMessageSequenceTrackerService
{
    public long GetNextSequenceNumber(Guid groupChatId);
}
