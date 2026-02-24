using realTimeMessagingWebApp.Controllers.ResponseModels;
using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;



namespace realTimeMessagingWebApp.Services
{
    public interface IChatService
    {
        public Task<ServiceResult> AssignChatAdmin(Guid chatId, Guid newAdminId);

        public Task<ServiceResult<Chat>> AddUserToChat(Guid chatId, Guid member);    

        public Task<ServiceResult<Chat>> AddUsersToChat(Guid chatId, ICollection<Guid> memberIds);

        public Task<ServiceResult> ChangeChatAdmin(Guid chatId, Guid memberId);

        public Task<ServiceResult> RemoveSelfFromChat(Guid chatId, Guid userId, bool? isAdmin = null);

        public Task<ServiceResult> RemoveOtherUserFromChat(Guid chatId, Guid memberId);

        public Task<ServiceResult> DeleteChat(Guid chatId); // will defo need auth, might also need to pass admin

        // is it really that bad to not pass in the entity and instead the GUID
        public Task<ServiceResult<Chat>> CreateAndAddMembersToChat(Chat chat, Guid Creator, Guid? admin, ICollection<Guid> memberIds);
        public Task<ServiceResult<ICollection<Chat>>> GetUserChats(Guid userId);
    }
}
