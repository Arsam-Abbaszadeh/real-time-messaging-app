using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.DtoMappers;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Controllers.ResponseModels;

namespace realTimeMessagingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        // a controller for managing user
        // for instance when logged getting all the chats for this user
        private readonly IUserService _userService = userService;


        // user CRUD endpoints
        [HttpPost("createNewUser")]
        public async Task<ActionResult<UserSummaryDto>> CreateNewUser([FromBody] CreateUserDto createUserDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            var newUser = UserDtoMapper.ToUserEntity(createUserDto);
            var creationResult = await _userService.CreateNewUser(newUser, createUserDto.Password);
            if (!creationResult.IsSuccess) 
            {
                return BadRequest(creationResult.Message);
            }

            var userSummary = UserDtoMapper.ToUserSummaryDto(newUser);
            return Ok(userSummary);
        }

        [HttpPost("login")]
        public async Task<ActionResult<RequestResponse>> LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            // TODO implement auth to pass through with login result
            // ATM this only validates a credentials and returns a response, it does not do any auth
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginResult = await _userService.LoginUser(loginUserDto.UserName, loginUserDto.Password);
            if (loginResult.IsSuccess) 
            {
                return Ok(new RequestResponse
                {
                     IsSuccess = true,
                     Message = $"User {loginUserDto.UserName} logged in successfully"
                });
            }
            return Unauthorized(new RequestResponse
            {
                IsSuccess = false,
                Message = loginResult.Message
            });
        }
    }
}
