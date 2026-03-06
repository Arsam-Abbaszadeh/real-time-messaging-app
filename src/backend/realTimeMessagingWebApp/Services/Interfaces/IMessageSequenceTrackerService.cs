namespace realTimeMessagingWebApp.Services;

public interface IMessageSequenceTrackerService
{
    public uint GetNextSequenceNumber(Guid chatId);
}
