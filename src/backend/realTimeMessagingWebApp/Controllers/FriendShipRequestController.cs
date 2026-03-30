using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.Controllers.ResponseModels;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Utilities;

namespace realTimeMessagingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FriendShipRequestController(IFriendShipRequestService friendShipRequestService) : ControllerBase
    {
        private readonly IFriendShipRequestService _friendShipRequestService = friendShipRequestService;

        [Authorize]
        [HttpPost("newfriendrequest")]
        public async Task<ActionResult<RequestResponse>> NewFriendRequest([FromBody] NewFriendRequestDto friendRequestDto)
        {
            var userId = User.GetUserId();

            var result = await _friendShipRequestService.MakeFriendShipRequest(userId, friendRequestDto.Friendid);
            if (result.IsSuccess)
            {
                return Ok(new RequestResponse
                {
                    IsSuccess = true,
                    Message = result.Message
                });
            }
            else
            {
                return BadRequest(new RequestResponse
                {
                    IsSuccess = false,
                    Message = result.Message
                });
            }
        }

        [Authorize]
        [HttpPatch("friendShipRequest/{requestId}")]
        public async Task<ActionResult<RequestResponse>> AcceptFriendRequest(Guid requestId)
        {
            var userId = User.GetUserId();

            var result = await _friendShipRequestService.AcceptFriendShipRequest(userId, requestId);

            if (result.IsSuccess)
            {
                return Ok(new RequestResponse
                {
                    IsSuccess = true,
                    Message = result.Message
                });
            }
            else
            {
                return BadRequest(new RequestResponse
                {
                    IsSuccess = false,
                    Message = result.Message
                });
            }
        }

        [Authorize]
        [HttpDelete("friendShipRequest/{requestId}")]
        public async Task<ActionResult<RequestResponse>> DeclineFriendRequest(Guid requestId)
        {
            var userId = User.GetUserId();

            var result = await _friendShipRequestService.DeclineFriendShipRequest(userId, requestId);
            if (result.IsSuccess)
            {
                return Ok(new RequestResponse
                {
                    IsSuccess = true,
                    Message = result.Message
                });
            }
            else
            {
                return BadRequest(new RequestResponse
                {
                    IsSuccess = false,
                    Message = result.Message
                });
            }
        }
    }
}
