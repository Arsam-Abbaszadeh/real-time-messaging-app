using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebApp.Data;
using realTimeMessagingWebApp.Entities;

namespace realTimeMessagingWebApp.Services.ResponseModels
{
    public class AuthService(Context dbContext) : IAuthService
    {
        readonly Context _context = dbContext;

        public async Task<ServiceResult> IsSelfActionOnGroupChat(Guid actionUserId, Guid targetUserId, Guid groupChatId)
        {
            var isMember = await _context.GroupChatConnectors
                .AnyAsync(gcc => gcc.GroupChatId == groupChatId && gcc.UserId == actionUserId);

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
                Message = "User is not taking action on themselves or is not a member of the group chat"
            };
        }

        public async Task<ServiceResult> UserIsGroupChatAdmin(Guid adminId, Guid groupChatId)
        {
            // this is where you use cache first and then db second

            var isAdmin = await _context.GroupChats.AnyAsync(gc => gc.GroupChatId == groupChatId && gc.GroupChatAdminId == adminId);

            if (isAdmin)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = "User is admin of the group chat"
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = "User is not admin of the group chat"
                };
            }
        }

        public async Task<ServiceResult> UserIsGroupChatMember(Guid memberId, Guid groupChatId)
        {
            var isMember = await _context.GroupChatConnectors
                .AnyAsync(gcc => gcc.GroupChatId == groupChatId && gcc.UserId == memberId);

            if (isMember)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User with Id {memberId} is member of the group chat"
                };
            }
            else
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"User with Id {memberId} is not member of the group chat"
                };
            }
        }

        public async Task<GroupChatAuthResult> GetGroupChatAuthStatus(Guid userId, Guid targetUserId, Guid groupChatId)
        {
            // Single query to check admin status
            var isAdmin = await _context.GroupChats
                .AnyAsync(gc => gc.GroupChatId == groupChatId && gc.GroupChatAdminId == userId);

            // If admin, they're automatically a member - no DB query needed
            var isMember = isAdmin 
                ? true 
                : await _context.GroupChatConnectors
                    .AnyAsync(gcc => gcc.GroupChatId == groupChatId && gcc.UserId == userId);

            var isSelfAction = userId == targetUserId;

            return new GroupChatAuthResult
            {
                IsAdmin = isAdmin,
                IsMember = isMember,
                IsSelfAction = isSelfAction
            };
        }
    }
}
