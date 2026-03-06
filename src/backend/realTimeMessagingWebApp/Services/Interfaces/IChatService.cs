using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;
using realTimeMessagingWebApp.Services.ArgumentOptions;



namespace realTimeMessagingWebApp.Services
{
    public interface IChatService
    {
        public Task<ServiceResult> AssignChatAdmin(Guid chatId, Guid newAdminId);

        public Task<ServiceResult> AddUserToChat(Guid chatId, Guid member);

        public Task<ServiceResult> AddUsersToChat(Guid chatId, ICollection<Guid> memberIds);

        public Task<ServiceResult> ChangeChatAdmin(Guid chatId, Guid memberId);

        public Task<ServiceResult> RemoveSelfFromChat(Guid chatId, Guid userId, bool? isAdmin = null);

        public Task<ServiceResult> RemoveOtherUserFromChat(Guid chatId, Guid memberId);

        public Task<ServiceResult> DeleteChat(Guid chatId);

        // Make data object for this and then make stored proc to retrieve.
        // also have correct error handeling for trying to access illegal sequence numbers
        public Task<ServiceResult> GetPaginatedChatHistory(ChatHistoryOptions options);

        public Task<ServiceResult<Chat>> CreateAndAddMembersToChat(Chat chat, Guid Creator, Guid? admin, ICollection<Guid> memberIds);
        public Task<ServiceResult<ICollection<Chat>>> GetUserChats(Guid userId);
    }
}
