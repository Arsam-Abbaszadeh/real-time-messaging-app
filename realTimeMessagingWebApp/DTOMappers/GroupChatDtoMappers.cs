using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Enums;

namespace realTimeMessagingWebApp.DTOMappers
{
    public static class GroupChatDtoMappers
    {
        public static GroupChat ToGroupChatEntity(CreateGroupChatDto createGroupChatDto)
        {
            return new GroupChat
            {
                GroupChatName = createGroupChatDto.GroupChatName,
                GroupChatType
            };
        }
    }
}
