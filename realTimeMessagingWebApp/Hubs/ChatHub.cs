using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using realTimeMessagingWebApp.Services;

namespace realTimeMessagingWebApp.Hubs;

[Authorize] // idk if this requires sepperate setup in Program.cs
public sealed class ChatHub(IAuthService authService, IMessageSequenceTrackerService sequenceService) : Hub
{
    readonly IAuthService _authService = authService;
    readonly IMessageSequenceTrackerService _sequenceService = sequenceService; // maybe should make singleton, to not have instance making overhead

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
            throw new HubException("You must be a member of the chat to join it");
        }

        if (!rooms.TryGetValue(roomName, out HashSet<string>? value))
        {
            value = [];
            rooms[roomName] = value;
        }

        value.Add(Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName); // I dont think we need to return anything
    }
    public async Task LeaveChatAsync(string roomName)
    {
        rooms[roomName].Remove(Context.ConnectionId);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    public async Task SendMessageToChat(string roomName, string user, string message)
    {
        if (!rooms.TryGetValue(roomName, out HashSet<string>? value) || !value.Contains(Context.ConnectionId))
        {
            throw new HubException("You must join the chat before sending messages");
        }

        var chatLock = chatLocks.GetOrAdd(roomName, _ => new SemaphoreSlim(1, 1));
        try
        {
            var chatGuid = Guid.Parse(roomName);
            var sequence = _sequenceService.GetNextSequenceNumber(chatGuid);
            await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message); // this is dealt with by front end

            // add to kafka queue with sequence
        }
        finally
        {
            chatLock.Release();
        }
    }
}


// actually lock messages now and then add kakfa and worker process to handle message saving and distribution