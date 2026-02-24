using realTimeMessagingWebAppInfra.Persistence.Data;
using System.Collections.Concurrent;

namespace realTimeMessagingWebApp.Services;

public class MessageSequenceTrackerService(Context dbContext) : IMessageSequenceTrackerService
{
    readonly Context _context = dbContext;
    // concurrent dictionary to handle multiple sequence numbers requested at once
    readonly static ConcurrentDictionary<Guid, ulong> ChatMessageSequenceNumber = []; // chatId, lastSequenceNumber

    // Method itself doesnt handle concurrent access well, as multiple users could get the same sequence number if they call this method at the same time for the same chat.
    // But the method caller (ChatHub) handles this by using a SemaphoreSlim per chat room to ensure that only one thread can access this method for a specific chat at a time.
    public ulong GetNextSequenceNumber(Guid chatId)
    {
        var sequenceLoaded = ChatMessageSequenceNumber.TryGetValue(chatId, out var lastSequenceNumber);
        if (sequenceLoaded)
        {
            lastSequenceNumber++;

            ChatMessageSequenceNumber[chatId] = lastSequenceNumber;
            return lastSequenceNumber;
        }
        else
        {
            var lastMessage = _context.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.SequenceNumber)
                .FirstOrDefault();

            lastSequenceNumber = lastMessage?.SequenceNumber is not null
            ? lastMessage.SequenceNumber + 1
            : 0;

            ChatMessageSequenceNumber[chatId] = lastSequenceNumber;
            return lastSequenceNumber;
        }
    }
}
