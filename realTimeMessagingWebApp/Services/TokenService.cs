using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using realTimeMessagingWebApp.Data;
using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;
using System.Security.Claims;
using System.Security.Claims;
using System.Security.Cryptography;

namespace realTimeMessagingWebApp.Services
{
    public class TokenService(Context context, IConfiguration configuration)
    : ITokenService
    {
        readonly IConfiguration _configuration = configuration; // service nopt registerd yet
        readonly Context _context = context;

        public string NewAccesssToken(string refreshToken)
        {
            throw new NotImplementedException();
        }

        public string NewRefreshToken()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> RevokeRefreshToken(string token)
        {
            var refreshToken = await _context.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token); // might not be worth including user just for the sake of error message
            if (refreshToken is null)
            {
                throw new InvalidOperationException("refresh token doesnt exist"); // idk if operation exception is the right one tbh
            }

            refreshToken.isValid = false;
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"Refresh Token for user {refreshToken.User.UserName} was revoked"
            };

        }

        public async Task<ServiceResult> RevokeRefreshTokenForUser(User user)
        {
            if (user.refreshTokens is null)
            {
                var refreshTokens = await _context.RefreshTokens
                    .Where(t => t.UserId == user.UserId && t.isValid)
                    .ToListAsync();

                foreach (var token in refreshTokens)
                {
                    token.isValid = false;
                }

                await _context.SaveChangesAsync();
            }

            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"All valid refresh tokens for user {user.UserName} have been revoked."
            };
        }

        #region tokenProvider

        string GenerateAccessToken(User user, DateTime expiration)
        {
            string secretKey = _configuration["Jwt:SecretKey"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("id", user.UserId.ToString()),
                new Claim("username", user.UserName)
                ]),
                Expires = expiration,
                SigningCredentials = credentials,
                Issuer = _configuration["Jwt:Issuer"], // understand requirement for Issuer and Audience and actually set
                Audience = _configuration["Jwt:Audience"],
            };

            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return token;
        }

        string GenerateRefreshToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        }

        #endregion
    }
}
