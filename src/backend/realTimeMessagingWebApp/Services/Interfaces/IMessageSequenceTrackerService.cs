namespace realTimeMessagingWebApp.Services;

public interface IMessageSequenceTrackerService
{
    public ulong GetNextSequenceNumber(Guid groupChatId);
}
