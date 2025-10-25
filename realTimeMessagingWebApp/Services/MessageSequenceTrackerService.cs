using System.Collections.Concurrent;
using realTimeMessagingWebApp.Data;

namespace realTimeMessagingWebApp.Services;

public class MessageSequenceTrackerService(Context dbContext) : IMessageSequenceTrackerService
{
    readonly Context _context = dbContext;
    // concurrent dictionary to handle ltiple sequence numbers requested at once
    readonly static ConcurrentDictionary<Guid, long> ChatMessageSequenceNumber = []; // groupChatId, lastSequenceNumber

    public long GetNextSequenceNumber(Guid groupChatId)
    {
        var sequenceLoaded = ChatMessageSequenceNumber.TryGetValue(groupChatId, out var lastSequenceNumber);
        if (sequenceLoaded)
        {
            lastSequenceNumber++;

            ChatMessageSequenceNumber[groupChatId] = lastSequenceNumber;
            return lastSequenceNumber;
        }
        else
        {
            var lastMessage = _context.Messages
                .Where(m => m.GroupChatId == groupChatId)
                .OrderByDescending(m => m.SequenceNumber)
                .FirstOrDefault();

            lastSequenceNumber = lastMessage?.SequenceNumber ?? -1; // 0 based index
            lastSequenceNumber++;

            ChatMessageSequenceNumber[groupChatId] = lastSequenceNumber;
            return lastSequenceNumber;
        }
    }
}
