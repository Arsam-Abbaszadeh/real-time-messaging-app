using realTimeMessagingWebApp.Data;
using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public class GroupChatService(Context context) : IGroupChatService
    {
        private readonly Context _context = context;

        public async Task<ServiceResult> AddUserToGroupChat(GroupChat groupChat, User member)
        {
            // Implementation for adding a user to a group chat
            throw new NotImplementedException();
        }
        public async Task<ServiceResult> ChangeGroupChatAdmin(GroupChat groupChat, User member)
        {
            // Implementation for changing the admin of a group chat
            throw new NotImplementedException();
        }
        public async Task<ServiceResult> RemoveUserFromGroupChat(GroupChat groupChat, User member)
        {
            // Implementation for removing a user from a group chat
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> CreateNewGroupChat(GroupChat groupChat, User Member)
        {
            
            return new ServiceResult
            {
                IsSuccess = true,
                Message = "New Group Chat Created",
            };
        }
    }
}
