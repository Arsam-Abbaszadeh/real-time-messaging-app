using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.Controllers.ResponseModels;
using realTimeMessagingWebApp.DTOMappers;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebApp.Services;

namespace realTimeMessagingWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ChatController(IChatService chatService, IAuthService authService) : ControllerBase
{
    readonly IChatService _chatService = chatService;
    readonly IAuthService _authService = authService;

    [Authorize]
    [HttpPost("newchat")]
    public async Task<ActionResult<RequestResponse>> CreateNewChat([FromBody] CreateChatDto chatDto)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated

        var newChat = ChatDtoMappers.ToChatEntity(chatDto);
        var chatResult = await _chatService.CreateAndAddMembersToChat(newChat, userId, chatDto.Admin, chatDto.ChatMembers);

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
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated
        var authResult = await _authService.UserIsChatAdmin(userId, chatId);

        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to add members to a chat"
            });
        }

        var addMembersResult = await _chatService.AddUsersToChat(chatId, newMemberIds);
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
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated

        var authResult = await _authService.UserIsChatAdmin(userId, chatId);
        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to delete a chat"
            });
        }

        var deleteResult = await _chatService.DeleteChat(chatId);
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
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated
        //var isSelfAction = _authService.IsSelfActionOnChat(userId, memberId, chatId);
        var userStatusResult = await _authService.GetChatAuthStatus(userId, memberId, chatId);

        if (userStatusResult.IsSelfAction)
        {
            var selfRemoveResult = await _chatService.RemoveSelfFromChat(chatId, userId, userStatusResult.IsAdmin);
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
            var removeOtherResult = await _chatService.RemoveOtherUserFromChat(chatId, memberId);
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
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!);
        var authResult = await _authService.UserIsChatAdmin(userId, chatId);
        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to change the admin of a chat"
            });
        }

        var changeAdminResult = await _chatService.ChangeChatAdmin(chatId, memberId);
        if (changeAdminResult.IsSuccess)
        {
            return Ok(new RequestResponse
            {
                IsSuccess = true,
                Message = changeAdminResult.Message
            });
        }
        else
        {
            return BadRequest(new RequestResponse
            {
                IsSuccess = false,
                Message = changeAdminResult.Message
            });
        }
    }

    [Authorize]
    [HttpGet("chatdsummaries")]
    public async Task<ActionResult<List<ChatSummaryDto>>> getChats()
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!);

        var chatsResult = await _chatService.GetUserChats(userId);
        if (!chatsResult.IsSuccess || chatsResult.Data is null)
        {
            return NotFound();
        }

        var chatSummaries = chatsResult.Data
            .Select(ChatDtoMappers.ToChatSummaryDto)
            .ToList();

        return Ok(chatSummaries);
    }
}
