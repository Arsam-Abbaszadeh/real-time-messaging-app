using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public interface ITokenService
    {

        public Task<ServiceResult> RevokeRefreshToken(string token);

        public Task<ServiceResult> RevokeRefreshTokenForUser(User user);

        public Task<AccessTokenResult> NewAccessToken(string refreshToken, DateTime expiration);

        public Task<TokenValidationServiceResult> ValidateAccessToken(string accessToken);

        public Task<string> NewRefreshToken(User user, DateTime expiration); // not sure what it needs to take in
    }
}
