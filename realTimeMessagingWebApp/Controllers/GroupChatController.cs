using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.DtoMappers;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Controllers.ResponseModels;
using realTimeMessagingWebApp.DTOMappers;

namespace realTimeMessagingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupChatController(IGroupChatService groupChatService, IUserService userService): ControllerBase // should maybe make chat lower case
    {
        private readonly IGroupChatService _groupChatService = groupChatService;
        private readonly IUserService _userService = userService;

        [HttpPost("newchat")]
        public Task<ActionResult<RequestResponse>> CreateNewGroupChat([FromBody] CreateGroupChatDto groupChatDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(new RequestResponse
            //    {
            //        IsSuccess = false,
            //        Message = "Bad Request, Couldnt create group chat"
            //    });
            //}

            var newGroupChat = GroupChatDtoMappers.ToGroupChatEntity(groupChatDto);
            var result = _groupChatService.CreateNewGroupChat(newGroupChat);

        }
    }
}
