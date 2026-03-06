using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.Services.ArgumentOptions;
using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebApp.DTOMappers
{
    public static class ChatDtoMappers
    {
        public static Chat ToChatEntity(CreateChatDto createChatDto)
        {
            return new Chat
            {
                ChatName = createChatDto.ChatName,
                ChatKind = createChatDto.ChatType,
            };
        }

        public static ChatSummaryDto ToChatSummaryDto(Chat chat)
        {
            return new ChatSummaryDto
            {
                ChatName = chat.ChatName,
                ChatId = chat.ChatId,
                ChatImageUrl = chat.ChatImageUrl,
            };
        }

        public static ChatHistoryOptions ToChatHistoryOptions(PaginatedChatHistoryOptionsDto optionsDto)
        {
            return new ChatHistoryOptions
            {
                ChatId = optionsDto.ChatId,
                StartMessageSequence = optionsDto.StartMessageSequence,
                EndMessageSequence = optionsDto.EndMessageSequence,
                EndFallBackToMaxInt = optionsDto.EndFallBackToMaxInt,
                EndMessageIsLast = optionsDto.EndMessageIsLast,
            };
        }
    }
}
