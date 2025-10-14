using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using realTimeMessagingWebApp.Services;

namespace realTimeMessagingWebApp.Hubs;

[Authorize] // idk if this requires sepperate setup in Program.cs
public sealed class ChatHub(IAuthService authService) : Hub
{
    readonly IAuthService _authService = authService;
    readonly static Dictionary<string, HashSet<string>> _rooms = []; // roomName, connectionIds

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

        if (!_rooms.ContainsKey(roomName))
        {
            _rooms[roomName] = new HashSet<string>();
        }

        _rooms[roomName].Add(Context.ConnectionId);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName); // I dont think we need to return anything
    }
    public async Task LeaveChatAsync(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
    }

    public async Task SendMessageToChat(string roomName, string user, string message)
    {
        if (!_rooms.ContainsKey(roomName) || !_rooms[roomName].Contains(Context.ConnectionId))
        {
            throw new HubException("You must join the chat before sending messages");
        }
        await Clients.Group(roomName).SendAsync("ReceiveMessage", user, message);
    }
}
