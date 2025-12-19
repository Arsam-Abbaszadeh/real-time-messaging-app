using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public interface IAuthService
    {
        public Task<ServiceResult> UserIsGroupChatAdmin(Guid adminId, Guid groupChatId);

        public Task<ServiceResult> UserIsGroupChatMember(Guid adminId, Guid groupChatId);

        public Task<ServiceResult> IsSelfActionOnGroupChat(Guid actionUserId, Guid targetUserId, Guid groupChatId);

        // TODO decide if you pass in bool isSelf or self id, how do we enforce auth usage
        public Task<GroupChatAuthResult> GetGroupChatAuthStatus(Guid userId, Guid targetUserId, Guid groupChatId);

    }
}
