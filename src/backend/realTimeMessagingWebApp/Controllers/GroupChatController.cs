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
public class ChatController(IGroupChatService groupChatService, IAuthService authService) : ControllerBase // should maybe make chat lower case
{
    readonly IGroupChatService _groupChatService = groupChatService;
    readonly IAuthService _authService = authService;

    [Authorize]
    [HttpPost("newchat")]
    public async Task<ActionResult<RequestResponse>> CreateNewGroupChat([FromBody] CreateGroupChatDto groupChatDto)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated

        var newGroupChat = GroupChatDtoMappers.ToGroupChatEntity(groupChatDto);
        var groupChatResult = await _groupChatService.CreateAndAddMembersToGroupChat(newGroupChat, userId, groupChatDto.Admin, groupChatDto.GroupChatMembers);

        if (groupChatResult.IsSuccess)
        {
            var groupChatSummary = GroupChatDtoMappers.ToGroupChatSummaryDto((GroupChat)groupChatResult.Data!);
            return Ok(new RequestResponse
            {
                IsSuccess = true,
                Message = groupChatResult.Message,
            });
        }
        else
        {
            return BadRequest(new RequestResponse
            {
                IsSuccess = false,
                Message = groupChatResult.Message
            });
        }
    }

    [Authorize]
    [HttpPost("{groupChatId}")]
    public async Task<ActionResult<RequestResponse>> AddMembersToGroupChat([FromRoute] Guid groupChatId, [FromBody] ICollection<Guid> newMemberIds)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated
        var authResult = await _authService.UserIsGroupChatAdmin(userId, groupChatId);

        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to add members to a chat"
            });
        }

        var addMembersResult = await _groupChatService.AddUsersToGroupChat(groupChatId, newMemberIds);
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
    [HttpDelete("{groupChatId}")]
    public async Task<ActionResult<RequestResponse>> DeleteGroupChat([FromRoute] Guid groupChatId)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated

        var authResult = await _authService.UserIsGroupChatAdmin(userId, groupChatId);
        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to delete a chat"
            });
        }

        var deleteResult = await _groupChatService.DeleteGroupChat(groupChatId);
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
    [HttpDelete("{groupChatid}/members/{memberId}")]
    public async Task<ActionResult<RequestResponse>> RemoveMemberFromGroupChat([FromRoute] Guid groupChatId, [FromRoute] Guid memberId)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated
        //var isSelfAction = _authService.IsSelfActionOnGroupChat(userId, memberId, groupChatId);
        var userStatusResult = await _authService.GetGroupChatAuthStatus(userId, memberId, groupChatId);

        if (userStatusResult.IsSelfAction)
        {
            var selfRemoveResult = await _groupChatService.RemoveSelfFromGroupChat(groupChatId, userId, userStatusResult.IsAdmin);
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
            var removeOtherResult = await _groupChatService.RemoveOtherUserFromGroupChat(groupChatId, memberId);
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
    [HttpPatch("{groupChatid}/members/{memberId}")]
    public async Task<ActionResult<RequestResponse>> ChangeGroupChatAdmin([FromRoute] Guid groupChatId, [FromRoute] Guid memberId)
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        var userId = Guid.Parse(userIdString!); // should not be null if token is validated
        var authResult = await _authService.UserIsGroupChatAdmin(userId, groupChatId);
        if (!authResult.IsSuccess)
        {
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = "You must be admin to change the admin of a chat"
            });
        }

        var changeAdminResult = await _groupChatService.ChangeGroupChatAdmin(groupChatId, memberId);
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
}
