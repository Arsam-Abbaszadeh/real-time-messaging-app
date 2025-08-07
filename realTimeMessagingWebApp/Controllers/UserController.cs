using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.DtoMappers;
using realTimeMessagingWebApp.Services;

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
        [HttpPost]
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



    }
}
