using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebAppInfra.Persistence.Data;
using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebApp.Services.ResponseModels
{
    public class AuthService(Context dbContext) : IAuthService
    {
        readonly Context _context = dbContext;

        public async Task<ServiceResult> IsSelfActionOnChat(Guid actionUserId, Guid targetUserId, Guid chatId)
        {
            var isMember = await _context.ChatConnectors
                .AsNoTracking()
                .AnyAsync(gcc => gcc.ChatId == chatId && gcc.UserId == actionUserId);

            var isSelf = actionUserId == targetUserId;

            if (isMember && isSelf)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = "User is taking action on themselves"
                };
            }

            return new ServiceResult
            {
                IsSuccess = false,
                Message = "User is not taking action on themselves or is not a member of the chat"
            };
        }

        public async Task<ServiceResult> UserIsChatAdmin(Guid adminId, Guid chatId)
        {
            // this is where you use cache first and then db second

            var isAdmin = await _context.Chats
                .AsNoTracking()
                .AnyAsync(gc => gc.ChatId == chatId && gc.ChatAdminId == adminId);

            if (isAdmin)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = "User is admin of the chat"
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "User is not admin of the chat (the chat might not exist)"
                };
            }
        }

        public async Task<ServiceResult> UserIsChatMember(Guid memberId, Guid chatId)
        {
            var isMember = await _context.ChatConnectors
                .AsNoTracking()
                .AnyAsync(gcc => gcc.ChatId == chatId && gcc.UserId == memberId);

            if (isMember)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User with Id {memberId} is member of the chat"
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with Id {memberId} is not member of the chat"
                };
            }
        }

        public async Task<ChatAuthResult> GetChatAuthStatus(Guid userId, Guid targetUserId, Guid chatId)
        {
            // Single query to check admin status
            var isAdmin = await _context.Chats
                .AsNoTracking()
                .AnyAsync(gc => gc.ChatId == chatId && gc.ChatAdminId == userId);

            // If admin, they're automatically a member - no DB query needed
            var isMember = isAdmin 
                ? true 
                : await _context.ChatConnectors
                    .AnyAsync(gcc => gcc.ChatId == chatId && gcc.UserId == userId);

            var isSelfAction = userId == targetUserId;

            return new ChatAuthResult
            {
                IsAdmin = isAdmin,
                IsMember = isMember,
                IsSelfAction = isSelfAction
            };
        }
    }
}
