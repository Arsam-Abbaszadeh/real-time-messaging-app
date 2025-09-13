using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using realTimeMessagingWebApp.Entities;

namespace realTimeMessagingWebApp.Auth.Tokens
{
    public sealed class TokenProvider(IConfiguration configuration)
    {
        readonly IConfiguration configuration = configuration; // service not yet actually injected

        public string CreateJwt(User user, DateTime expiration)
        {
            string secretKey = configuration["Jwt:SecretKey"];
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256); // The encryption algo doesnt matter too much 

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim("id", user.UserId.ToString()),
                    new Claim("username", user.UserName)
                ]),
                Expires = expiration,
                SigningCredentials = credentials,
                Issuer = configuration["Jwt:Issuer"], // understand requirement for Issuer and Audience and actually set
                Audience = configuration["Jwt:Audience"],
            };

            var handler = new JsonWebTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);

            return token;
        }
    }
}
