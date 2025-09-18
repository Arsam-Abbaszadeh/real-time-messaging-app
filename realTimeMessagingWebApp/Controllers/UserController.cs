using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.DtoMappers;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Controllers.ResponseModels;
using realTimeMessagingWebApp.Entities;

namespace realTimeMessagingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService, ITokenService tokenService) : ControllerBase
    {
        // a controller for managing user
        // for instance when logged getting all the chats for this user

        readonly IUserService _userService = userService;
        readonly ITokenService _tokenService = tokenService;

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
            return Ok(userSummary); // tihis should then somehow lead into login flow after new user got created
        }

        [HttpPost("login")]
        public async Task<ActionResult<RequestResponse>> LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginResult = await _userService.LoginUser(loginUserDto.UserName, loginUserDto.Password);
            if (loginResult.IsSuccess) 
            {
                var refreshToken = await _tokenService.NewRefreshToken((User)loginResult.Data);

                Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddHours(1), // this should be stored not hard coded. probs inject app settings and we pass around the configurations
                    SameSite = SameSiteMode.Strict // might be lax
                });

                var accessToken = await _tokenService.NewAccessToken(refreshToken);

                return Ok(new RequestResponse
                {
                    IsSuccess = true,
                    Message = $"User {loginUserDto.UserName} logged in successfully",
                    AccessToken = accessToken
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
