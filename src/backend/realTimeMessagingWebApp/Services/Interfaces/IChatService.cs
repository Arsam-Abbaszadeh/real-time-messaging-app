using realTimeMessagingWebApp.Services.ArgumentOptions;
using realTimeMessagingWebApp.Services.ResponseModels;
using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebApp.Services.Interfaces;

public interface IChatService
{
    public Task<ServiceResult<IList<Message>>> GetPaginatedChatHistory(ChatHistoryOptions options);

    public Task<ServiceResult<IList<Message>>> GetTopNChatMessages(Guid chatId, int range);

    public Task<ServiceResult<IList<Chat>>> GetUserChats(Guid userId);
}
