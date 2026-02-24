using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services;

public interface IAuthService
{
    public Task<ServiceResult> UserIsChatAdmin(Guid adminId, Guid chatId);

    public Task<ServiceResult> UserIsChatMember(Guid adminId, Guid chatId);

    public Task<ServiceResult> IsSelfActionOnChat(Guid actionUserId, Guid targetUserId, Guid chatId);

    // TODO decide if you pass in bool isSelf or self id, how do we enforce auth usage
    public Task<ChatAuthResult> GetChatAuthStatus(Guid userId, Guid targetUserId, Guid chatId);

}
