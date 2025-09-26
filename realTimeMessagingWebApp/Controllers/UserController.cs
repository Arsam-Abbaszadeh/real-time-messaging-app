﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using realTimeMessagingWebApp.DTOs;
using realTimeMessagingWebApp.DtoMappers;
using realTimeMessagingWebApp.Services;
using realTimeMessagingWebApp.Controllers.ResponseModels;
using realTimeMessagingWebApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace realTimeMessagingWebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService, ITokenService tokenService, IConfiguration configuration) : ControllerBase
    {
        // a controller for managing user
        // for instance when logged getting all the chats for this user

        readonly IUserService _userService = userService;
        readonly ITokenService _tokenService = tokenService;
        readonly IConfiguration _configuration = configuration;

        const string RefreshTokenName = "refreshToken";

        // user CRUD endpoints
        [HttpPost("createnewuser")]
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
            return Ok(userSummary); // this should then somehow lead into login flow after new user got created
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserAccessResultDto>> LoginUser([FromBody] LoginUserDto loginUserDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var loginResult = await _userService.LoginUser(loginUserDto.UserName, loginUserDto.Password);
            if (loginResult.IsSuccess) 
            {
                var refreshExpiration = DateTime.Now.AddDays(_configuration.GetValue<int>("Jwt:RefreshExpiration"));
                var refreshToken = await _tokenService.NewRefreshToken((User)loginResult.Data, refreshExpiration);

                Response.Cookies.Append(RefreshTokenName, refreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    Expires = DateTime.Now.AddHours(1),
                    SameSite = SameSiteMode.Lax // might be strict
                });

                var accessExpiration = DateTime.Now.AddMinutes(_configuration.GetValue<int>("Jwt:AccessExpiration")); // configure for UTC
                var accessTokenResult = await _tokenService.NewAccessToken(refreshToken, accessExpiration);


                return Ok(new UserAccessResultDto
                {
                    IsSuccessful = true,
                    Message = $"User {loginUserDto.UserName} logged in successfully",
                    AccessToken = accessTokenResult.AccessToken
                });
            }

            return Unauthorized(new UserAccessResultDto
            {
                IsSuccessful = false,
                Message = loginResult.Message
            });
        }

        // actually test this idk if this endpoint works

        [HttpGet("refreshaccesstokentoken")]
        public async Task<ActionResult<UserAccessResultDto>> RefreshAccessToken() 
        {
            if (!ModelState.IsValid) // TODO could this be done with middleware instead?
            {
                return BadRequest(ModelState);
            }

            var authHeaderStart = "Bearer "; // TODO should Bearer be included when we send cookie initially
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader == null || !authHeader.StartsWith(authHeaderStart))
            {
                return BadRequest("Access token required in Authorization header");
            }

            var expiredAccessToken = authHeader[authHeaderStart.Length..].Trim(); 
            var validationResult = await _tokenService.ValidateAccessToken(expiredAccessToken);

            if (validationResult.validationResult)
            {
                if ((bool)validationResult.validationSuccess!)
                {
                    return BadRequest("Access token is still valid, no need to refresh it");
                }

                var refreshToken = Request.Cookies[RefreshTokenName]; // is worth checking this before validating access token?

                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized("Refresh token is required to refresh access token");
                }

                var expiration = DateTime.Now.AddMinutes(_configuration.GetValue<int>("Jwt:AccessExpiration"));
                var accessTokenResult = await _tokenService.NewAccessToken(refreshToken, expiration); //actually get user

                try
                {
                     if (accessTokenResult.ValidRefreshToken)
                    {
                        return Ok(new UserAccessResultDto
                        {
                            IsSuccessful = true,
                            Message = "new access token generated successfully",
                            AccessToken = accessTokenResult.AccessToken
                        });
                    }

                } 
                catch (InvalidOperationException ex)
                {
                    return Unauthorized(ex.Message);
                }
            }

            // token validation failed
            return Unauthorized("Access token is invalid" + (validationResult.message != null ? $": {validationResult.message}" : ""));
        }
    }
}
