using realTimeMessagingWebAppInfra.Storage.Constants;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using realTimeMessagingWebApp.Configurations;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebAppInfra.Storage.Services;
using realTimeMessagingWebAppInfra.Storage.Utilities;
using Microsoft.Extensions.Options;

namespace realTimeMessagingWebApp.Hubs;

[Authorize]
public sealed class ChatHub(
    IAuthService authService,
    IMessageSequenceTrackerService sequenceService,
    IKafkaProducerService kafkaProducerService,
    IObjectStorageService ObjectStorageService,
    IOptions<KafkaConfigurations> KafkaConfigurations
) : Hub
{
    readonly IAuthService _authService = authService;
    readonly IMessageSequenceTrackerService _sequenceService = sequenceService; // maybe should make singleton, to not have instance making overhead
    readonly IKafkaProducerService _kafkaProducerService = kafkaProducerService;
    readonly IObjectStorageService _objectStorageService = ObjectStorageService;
    readonly KafkaConfigurations _kafkaConfigurations = KafkaConfigurations.Value;

    readonly static ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> rooms = []; // chatId, (signalR connectionIds, filler byte value)
    readonly static ConcurrentDictionary<string, SemaphoreSlim> chatLocks = []; // chatId, semaphore

    public async Task JoinChatAsync(Guid chatId)
    {
        var strChatId = chatId.ToString();
        if (!UserHasJoinedChat(strChatId))
        {
            var userIdString = Context.User?.Claims.First(c => c.Type == "id")?.Value;
            var userId = Guid.Parse(userIdString!); // should not be null if token is validated
            var isMember = await _authService.UserIsChatMember(userId, chatId);
            if (!isMember.IsSuccess)
            {
                throw new HubException("You must be a member of the chat to join it"); // does this return a bad request type thing to the frontend?
            }

            if (!rooms.TryGetValue(strChatId, out ConcurrentDictionary<string, byte>? value))
            {
                value = [];
                rooms[strChatId] = value;
            }

            value.TryAdd(strChatId, 0);
            await Groups.AddToGroupAsync(Context.ConnectionId, strChatId);
        }
    }

    public async Task LeaveChatAsync(Guid chatId)
    {
        var strChatId = chatId.ToString();
        ThrowIfUserNotInChat(strChatId, "You must join the chat before trying to leave it");
        rooms[strChatId].TryRemove(Context.ConnectionId, out _);
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, strChatId);
    }

    public async Task GetPreSignedUrlForChatImageUpload(ImageDetailsForUploadUrlDto imageDetails)
    {
        var chatId = imageDetails.ChatId.ToString();
        ThrowIfUserNotInChat(chatId, "You must join the chat before trying to upload files to it");

        var objectKey = ObjectStorageUtilities.GenerateObjectKeyForChatFile(
            imageDetails.UserId, imageDetails.ChatId, imageDetails.FileExtension);

        var userId = Guid.Parse(Context.User?.Claims.First(c => c.Type == "id")?.Value);
        var presignedUrl = await _objectStorageService.CreateObjectUrlForClientUploadAsync(
            BucketKeys.Private, objectKey, imageDetails.FileType);

        await Clients.Clients(Context.ConnectionId)
            .SendAsync("ReceivePreSignedUrlForChatImageUpload", new ImageAccessDetailsDto
            {
                PresignedUrl = presignedUrl,
                ObjectKey = objectKey
            });
    }

    //What the hell is the user argument
    public async Task SendMessageToChat(UserChatMessageRecieveDto messageContents) // add all the other data that needs to be sent by front end when tring to send a message
    {
        var chatId = messageContents.ChatId.ToString();
        ThrowIfUserNotInChat(chatId, "You must join the chat before sending messages");

        var chatLock = chatLocks.GetOrAdd(chatId, _ => new SemaphoreSlim(1, 1));
        await chatLock.WaitAsync();
        try
        {
            var sequence = _sequenceService.GetNextSequenceNumber(messageContents.ChatId);
            var userId = Context.User?.Claims.First(c => c.Type == "id")?.Value;
            //dont think I need to be the entire message content to all users but no harm in sending a bit more data
            await Clients.Group(chatId).SendAsync("ReceiveMessage", userId, messageContents); // this is dealt with by front end
            chatLock.Release();
            await _kafkaProducerService.ProduceAsync(_kafkaConfigurations.Topic, _kafkaConfigurations.Key, messageContents);
        }
        finally
        {
            if (chatLock.CurrentCount == 0)
            {
                chatLock.Release();
            }
        }
    }

    #region helpers
    void ThrowIfUserNotInChat(string chatId, string errorMessage = "You must join the chat before performing this operation")
    {
        if (!UserHasJoinedChat(chatId))
        {
            throw new HubException(errorMessage);
        }
    }

    bool UserHasJoinedChat(string chatId) 
    {
        return rooms.TryGetValue(chatId, out ConcurrentDictionary<string, byte>? value) || (value is not null && !value.ContainsKey(Context.ConnectionId));
    }
    #endregion
}