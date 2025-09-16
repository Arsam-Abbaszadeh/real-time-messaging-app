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

        public async Task<string> NewAccesssToken(string refreshToken)
        {
            var validRefreshToken = await _context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (validRefreshToken?.isValid == true)
            {
                // expiration date may be inconsistent with other stuff
                var newAccessToken = GenerateAccessToken(validRefreshToken.User, DateTime.Today.AddDays(1)); // need a mechanism for loading the right expiration dates
                return newAccessToken;
            }
            
            throw new InvalidOperationException("trying get new access token using expired or unknown refresh token"); // not exactly sure what exeception type should be thrown tbh
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
            // assuming user is valid, might be a dumb approach
            await _context.Entry(user)
                .Collection(u => u.refreshTokens)
                .Query()
                .Where(t => t.isValid)
                .LoadAsync();

            if (!user.refreshTokens.Any())
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User  {user.UserName} doesn't have any active refresh tokens"
                };
            }
            
            foreach (var token in user.refreshTokens)
            {
                token.isValid = false;
            }

            await _context.SaveChangesAsync();

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
