using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.Services.ResponseModels;
using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebAppInfra.Storage.Constants;
using realTimeMessagingWebAppInfra.Storage.Services;

namespace realTimeMessagingWebApp.Services;

public class DtoAssemblerService(IObjectStorageService objectStorageService) : IDtoAssemblerService
{
    readonly IObjectStorageService _objectStorageService = objectStorageService;
    public async Task<ServiceResult<IList<MessageDto>>> AssembleMessageDtosFromMessages(IList<Message> messages, bool includeAttachmentUrl = true)
    {
        var messageDtos = new List<MessageDto>();
        foreach (var message in messages)
        {
            var messageDto = new MessageDto 
            {
                ChatId = message.ChatId,
                MessageSenderId = message.SenderId,
                MessageIsEdited = message.IsEdited,
                MessageSequenceNumber = message.SequenceNumber,
                MessageContent = message.MessageContent,
                MessageId = message.MessageId,
                MessageSentAt = message.SentAt,
            };

            if (message.Attachments.Count > 0) 
            {
                foreach (var attachment in message.Attachments) 
                {
                    var bucket = (BucketKeys)attachment.AttachementBucketKey;
                    var url = default(string);

                    if (includeAttachmentUrl)
                    {
                        url = await _objectStorageService.GetObjectUrlForClientRenderingAsync(bucket, attachment.AttachmentObjectKey);
                    }

                    var AttachementDto = new AttachementDto
                    {
                        MessageId = attachment.MessageId,
                        AttachementId = attachment.MessageAttachmentId,
                        AttachmentMimeType = attachment.AttachmentMimeType,
                        AttachmentUrl = url ?? ""
                    };

                    messageDto.MessageAttachments.Add(AttachementDto);
                }
            }

            messageDtos.Add(messageDto);
        }

        return new ServiceResult<IList<MessageDto>> 
        { 
            IsSuccess = true,
            Data = messageDtos,
        };
    }
}
