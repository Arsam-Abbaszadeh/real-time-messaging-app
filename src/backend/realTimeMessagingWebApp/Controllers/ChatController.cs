using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.Controllers.ResponseModels;
using realTimeMessagingWebApp.DTOMappers;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.Controllers.QureyParamObjects;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Utilities;

namespace realTimeMessagingWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController(IChatUserService chatUserService, IAuthService authService, IChatService chatService) : ControllerBase
{
    readonly IChatUserService _chatUserService = chatUserService;
    readonly IAuthService _authService = authService;
    readonly IChatService _chatService = chatService;

    [Authorize]
    [HttpPost("newchat")]
    public async Task<ActionResult<RequestResponse>> CreateNewChat([FromBody] CreateChatDto chatDto)
    {
        var userId = User.GetUserId();

        var newChat = ChatDtoMappers.ToChatEntity(chatDto);
        var chatResult = await _chatUserService.CreateAndAddMembersToChat(newChat, userId, chatDto.Admin, chatDto.ChatMembers);

        if (chatResult.IsSuccess)
        {
            var chatSummary = ChatDtoMappers.ToChatSummaryDto(chatResult.Data!);
            return Ok(new RequestResponse
            {
                IsSuccess = true,
                Message = chatResult.Message,
            });
        }
        else
        {
            return BadRequest(new RequestResponse
            {
                IsSuccess = false,
                Message = chatResult.Message
            });
        }
    }

    [Authorize]
    [HttpPost("{chatId}")]
    public async Task<ActionResult<RequestResponse>> AddMembersToChat([FromRoute] Guid chatId, [FromBody] ICollection<Guid> newMemberIds)
    {
        var userId = User.GetUserId();
        var authResult = await _authService.UserIsChatAdmin(userId, chatId);

        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to add members to a chat"
            });
        }

        var addMembersResult = await _chatUserService.AddUsersToChat(chatId, newMemberIds);
        if (addMembersResult.IsSuccess)
        {
            return Ok(new RequestResponse
            {
                IsSuccess = true,
                Message = addMembersResult.Message
            });
        }
        else
        {
            return BadRequest(new RequestResponse
            {
                IsSuccess = false,
                Message = addMembersResult.Message
            });
        }
    }


    // Fix to only assign group members as admins not just friends
    [Authorize]
    [HttpDelete("{chatId}")]
    public async Task<ActionResult<RequestResponse>> DeleteChat([FromRoute] Guid chatId)
    {
        var userId = User.GetUserId();

        var authResult = await _authService.UserIsChatAdmin(userId, chatId);
        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to delete a chat"
            });
        }

        var deleteResult = await _chatUserService.DeleteChat(chatId);
        if (deleteResult.IsSuccess)
        {
            return Ok(new RequestResponse
            {
                IsSuccess = true,
                Message = deleteResult.Message
            });
        }
        else
        {
            return BadRequest(new RequestResponse
            {
                IsSuccess = false,
                Message = deleteResult.Message
            });
        }
    }

    [Authorize]
    [HttpDelete("{chatId}/members/{memberId}")]
    public async Task<ActionResult<RequestResponse>> RemoveMemberFromChat([FromRoute] Guid chatId, [FromRoute] Guid memberId)
    {
        var userId = User.GetUserId();
        //var isSelfAction = _authService.IsSelfActionOnChat(userId, memberId, chatId);
        var userStatusResult = await _authService.GetChatAuthStatus(userId, memberId, chatId);

        if (userStatusResult.IsSelfAction)
        {
            var selfRemoveResult = await _chatUserService.RemoveSelfFromChat(chatId, userId, userStatusResult.IsAdmin);
            if (selfRemoveResult.IsSuccess)
            {
                return Ok(new RequestResponse
                {
                    IsSuccess = true,
                    Message = selfRemoveResult.Message
                });
            }
            else
            {
                return BadRequest(new RequestResponse
                {
                    IsSuccess = false,
                    Message = selfRemoveResult.Message
                });
            }
        }
        else if (userStatusResult.IsAdmin) // user is admin and removing other user
        {
            var removeOtherResult = await _chatUserService.RemoveOtherUserFromChat(chatId, memberId);
            if (removeOtherResult.IsSuccess)
            {
                return Ok(new RequestResponse
                {
                    IsSuccess = true,
                    Message = removeOtherResult.Message
                });
            }
            else
            {
                return BadRequest(new RequestResponse
                {
                    IsSuccess = false,
                    Message = removeOtherResult.Message
                });
            }
        }
        else // not self action and not admin
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to remove other members from a chat"
            });
        }
    }

    [Authorize]
    [HttpPatch("{chatId}/members/{memberId}")]
    public async Task<ActionResult<RequestResponse>> ChangeChatAdmin([FromRoute] Guid chatId, [FromRoute] Guid memberId)
    {
        var userId = User.GetUserId();
        var authResult = await _authService.UserIsChatAdmin(userId, chatId);
        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to change the admin of a chat"
            });
        }

        var changeAdminResult = await _chatUserService.ChangeChatAdmin(chatId, memberId);
        if (changeAdminResult.IsSuccess)
        {
            return Ok(new RequestResponse
            {
                IsSuccess = true,
                Message = changeAdminResult.Message
            });
        }

        return BadRequest(new RequestResponse
        {
            IsSuccess = false, 
            Message = changeAdminResult.Message
        });
    }

    [Authorize]
    [HttpGet("summaries")]
    public async Task<ActionResult<ChatSummaryDto>> GetChatSummaries()
    {
        var userId = User.GetUserId();
        var chatResult = await _chatService.GetUserChats(userId);
        var chatSummaries = chatResult.Data!.Select(c => ChatDtoMappers.ToChatSummaryDto(c));
         
        return Ok(chatSummaries);
    }
    
    [Authorize]
    [HttpGet("{chatId}/messages")]
    public async Task<ActionResult<MessageDto>> GetMessages([FromRoute] Guid chatId, [FromQuery] int? messageCount, [FromQuery] PaginatedChatHistoryOptionsQuery options)
    {
        var userId = User.GetUserId();
        var validOptions = options.validate();
        if (validOptions && messageCount is not null)
        {
            return BadRequest("Must choose between messageCount and pagination options");
        }
        
        if (validOptions) {
            var paginationOptions = _chatService.GetPaginatedChatHistory(ChatDtoMappers.ToChatHistoryOptions(chatId, options));
        }
        if (messageCount is not null)
        {
            var messageResult = await _chatService.GetTopNChatMessages(chatId, messageCount.Value);
            if (messageResult.IsSuccess)
            {
                return Ok(messageResult.Data!);
            }
        }
        
        // TODO: better error message
        return BadRequest("Need either messageCount or pagination params correctly");
    }
}
