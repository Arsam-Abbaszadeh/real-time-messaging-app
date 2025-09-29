using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.DtoMappers;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Controllers.ResponseModels;
using realTimeMessagingWebApp.DTOMappers;
using System.Reflection.Metadata.Ecma335;
using realTimeMessagingWebApp.Entities;

namespace realTimeMessagingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController(IGroupChatService groupChatService, IAuthService authService) : ControllerBase // should maybe make chat lower case
    {
        private readonly IGroupChatService _groupChatService = groupChatService;
        private readonly IAuthService _authService = authService;

        [HttpPost("newchat")]
        public async Task<ActionResult<RequestResponse>> CreateNewGroupChat([FromBody] CreateGroupChatDto groupChatDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RequestResponse
                {
                    IsSuccess = false,
                    Message = "Invalid data"
                });
            }

            var userIdString = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            var userId = Guid.Parse(userIdString!); // should not be null if token is validated

            var newGroupChat = GroupChatDtoMappers.ToGroupChatEntity(groupChatDto);
            var groupChatResult = await _groupChatService.CreateAndAddMembersToGroupChat(newGroupChat, userId, groupChatDto.GroupChatMembers);

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
}
