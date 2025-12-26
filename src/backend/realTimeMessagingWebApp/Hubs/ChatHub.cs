using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using realTimeMessagingWebApp.Configurations;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.Services;

namespace realTimeMessagingWebApp.Hubs;

[Authorize] // idk if this requires sepperate setup in Program.cs
public sealed class ChatHub(
    IAuthService authService,
    IMessageSequenceTrackerService sequenceService,
    IKafkaProducerService kafkaProducerService,
    KafkaConfigurations KafkaConfigurations
) : Hub
{
    readonly IAuthService _authService = authService;
    readonly IMessageSequenceTrackerService _sequenceService = sequenceService; // maybe should make singleton, to not have instance making overhead
    readonly IKafkaProducerService _kafkaProducerService = kafkaProducerService;
    readonly KafkaConfigurations _kafkaConfigurations = KafkaConfigurations;

    readonly static ConcurrentDictionary<string, HashSet<string>> rooms = []; // roomName, connectionIds
    readonly static ConcurrentDictionary<string, SemaphoreSlim> chatLocks = []; // roomName, semaphore

    public async Task JoinChatAsync(string roomName)
    {
        var userIdString = Context.User?.Claims.First(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated
        var roomGuid = Guid.Parse(roomName);

        var isMember = await _authService.UserIsGroupChatMember(userId, roomGuid);
        if (!isMember.IsSuccess)
        {
            throw new HubException("You must be a member of the chat to join it"); // does this return a bad request type thing to the frontend?
        }

        if (!rooms.TryGetValue(roomName, out HashSet<string>? value))
        {
            value = [];
            rooms[roomName] = value;
        }

        value.Add(Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }

    public async Task LeaveChatAsync(string roomName)
    {
        rooms[roomName].Remove(Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    // What the hell is the user argument
    public async Task SendMessageToChat(string roomName, string user, UserChatMessageRecieveDto messageContents) // add all the other data that needs to be sent by front end when tring to send a message
    {
        if (!rooms.TryGetValue(roomName, out HashSet<string>? value) || !value.Contains(Context.ConnectionId))
        {
            throw new HubException("You must join the chat before sending messages");
        }

        var chatLock = chatLocks.GetOrAdd(roomName, _ => new SemaphoreSlim(1, 1));
        await chatLock.WaitAsync();
        try
        {
            var chatGuid = Guid.Parse(roomName);
            var sequence = _sequenceService.GetNextSequenceNumber(chatGuid);
            // dont think I need to be the entire message content to all users but no harm in sending a bit more data
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, messageContents); // this is dealt with by front end
            // This doesnte actually need to be done in the lock but code is easier to read this way
            await _kafkaProducerService.ProduceAsync(_kafkaConfigurations.Topic, _kafkaConfigurations.Key, messageContents);
        }
        finally
        {
            chatLock.Release();
        }
    }
}