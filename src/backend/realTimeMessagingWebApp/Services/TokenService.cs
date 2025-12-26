using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using realTimeMessagingWebAppInfra.Persistence.Data;

namespace realTimeMessagingWebApp.Services
{
    public class TokenService(Context context, IConfiguration configuration)
    : ITokenService
    {
        readonly IConfiguration _configuration = configuration; // should defop change environment variables, but not everything needs to be in env vars some can be in app settings and in that figure out how to snapshot appsettings
        readonly Context _context = context;

        public async Task<AccessTokenResult> NewAccessToken(string refreshToken, DateTime expiration)
        {
            var validRefreshToken = await _context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (validRefreshToken?.isValid == true)
            {
                if (validRefreshToken.ExpirationUtc > DateTime.UtcNow)
                {
                    var newAccessToken = GenerateAccessToken(validRefreshToken.User, expiration); // need a mechanism for loading the right expiration dates
                    return new AccessTokenResult
                    {
                        AccessToken = newAccessToken,
                        ValidRefreshToken = true,
                        Message = "New access token granted"
                    };
                }

                validRefreshToken.isValid = false;
                await _context.SaveChangesAsync();
            }

            return new AccessTokenResult
            {
                ValidRefreshToken = false,
                Message = "Cannot get new access token using invalid refresh token"
            };
        }

        public async Task<string> NewRefreshToken(User user, DateTime expiration) // assumes user is valid
        {
            var token = GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = user.UserId,
                ExpirationUtc = expiration,
                isValid = true
            };

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return token;
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
                .Collection(u => u.RefreshTokens)
                .Query()
                .Where(t => t.isValid)
                .LoadAsync();

            if (!user.RefreshTokens.Any())
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"User  {user.UserName} doesn't have any active refresh tokens"
                };
            }
            
            foreach (var token in user.RefreshTokens)
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

        public async Task<TokenValidationServiceResult> ValidateAccessToken(string accessToken)
        {
            string secretKey = _configuration["Jwt:SecretKey"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var handler = new JsonWebTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero, // optional: to reduce the allowed clock skew time
                ValidateLifetime = true
            };

            // doesnt throw exception, just returns the exception in the result object
            var result = await handler.ValidateTokenAsync(accessToken, tokenValidationParameters);
            if (result.IsValid)
            {
                return new TokenValidationServiceResult
                {
                    validationResult = true,
                    validationSuccess = true,
                    message = "Token is valid"
                };
            }
            else
            {
                return new TokenValidationServiceResult
                {
                    validationResult = true,
                    validationSuccess = false,
                    message = result.Exception?.Message ?? "Token is invalid"
                };
            }
        }


        #region tokenProvider

        string GenerateAccessToken(User user, DateTime expiration)
        {
            string secretKey = _configuration["Jwt:SecretKey"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity( // TODO will eventually need dynamic claims probably
                [
                    new Claim("id", user.UserId.ToString()),
                    new Claim("username", user.UserName)
                ]),
                Expires = expiration,
                SigningCredentials = credentials,
                Issuer = _configuration["Jwt:Issuer"],
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
