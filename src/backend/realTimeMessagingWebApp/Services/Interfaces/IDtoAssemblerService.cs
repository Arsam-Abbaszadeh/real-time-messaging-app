using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.Services.ResponseModels;
using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebApp.Services;

public interface IDtoAssemblerService
{
    Task<ServiceResult<IList<MessageDto>>> AssembleMessageDtosFromMessages(IList<Message> messages, bool includeAttachmentUrl = true);
}
