using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebApp.DTOMappers
{
    public static class GroupChatDtoMappers
    {
        public static GroupChat ToGroupChatEntity(CreateGroupChatDto createGroupChatDto)
        {
            return new GroupChat
            {
                GroupChatName = createGroupChatDto.GroupChatName,
                ChatType = createGroupChatDto.GroupChatType,
            };
        }

        public static GroupChatSummaryDto ToGroupChatSummaryDto(GroupChat groupChat)
        {
            return new GroupChatSummaryDto
            {
                GroupChatName = groupChat.GroupChatName,
                GroupChatId = groupChat.GroupChatId,
                GroupChatImageUrl = groupChat.GroupChatImageUrl,
            };
        }
    }
}
