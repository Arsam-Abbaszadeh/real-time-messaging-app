using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebAppData.Entities;

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
                GroupChatCreationDate = groupChat.CreationDate,
                GroupChatName = groupChat.GroupChatName,
                GroupChatId = groupChat.GroupChatId,
            };
        }
    }
}
