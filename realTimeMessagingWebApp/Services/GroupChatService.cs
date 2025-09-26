using realTimeMessagingWebApp.Data;
using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Enums;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public class GroupChatService(Context context) : IGroupChatService
    {
        private readonly Context _context = context;

        public async Task<ServiceResult> AddUserToGroupChat(GroupChat groupChat, User member)
        {

            
        }

        public async Task <ServiceResult> AddUserToGroupChat(GroupChat groupChat, ICollection<User> members)
        {
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

        public Task<ServiceResult> CreateNewGroupChat(GroupChat groupChat, Guid Creator, Guid? Admin)
        {
            groupChat.CreationDate = DateTime.UtcNow;
            groupChat.GroupChatCreatorId = Creator;
            groupChat.GroupChatAdminId = Admin ?? Creator;
            groupChat.GroupChatId = Guid.NewGuid();
            GroupChatType type = groupChat.ChatType;


        }

        public Task<ServiceResult> DeleteGroupChat(GroupChat groupChat)
        {
            throw new NotImplementedException();
        }
    }
}
