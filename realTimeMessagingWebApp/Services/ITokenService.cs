using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public interface ITokenService
    {

        public Task<ServiceResult> RevokeRefreshToken(string token);

        public Task<ServiceResult> RevokeRefreshTokenForUser(User user);

        public Task<string> NewAccesssToken(string refreshToken);

        public string NewRefreshToken(); // not sure what it needs to take in

    }
}
