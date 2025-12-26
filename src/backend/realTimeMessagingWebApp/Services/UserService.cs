using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebApp.Auth;
using realTimeMessagingWebAppInfra.Persistence.Data;
using realTimeMessagingWebAppInfra.Persistence.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;
using System.Reflection;

namespace realTimeMessagingWebApp.Services
{
    public class UserService(Context context) : IUserService
    {
        private readonly Context _context = context;

        public async Task<ServiceResult> CreateNewUser(User user, string password)
        {
            _ = user ?? throw new ArgumentNullException(nameof(user));// should never really happen

            var userExists = await _context.Users.AnyAsync(u => u.UserName == user.UserName);
            if (userExists)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"The username {user.UserName} already exists"
                };
            }
            user.UserId = Guid.NewGuid();
            user.SignUpDate = DateTime.UtcNow;
            user.PasswordHash = AuthUtils.HashPassword(password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = "New User Saved to Db"
            };   
        }

        public async Task<ServiceResult> LoginUser(string userName, string password)
        {
            var loginUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (loginUser is null)
            {
                return new ServiceResult
                {
                    IsSuccess = false,
                    Message = $"The username {userName} does not exists"
                };
            }

            if (AuthUtils.VerifyHashedPassword(loginUser.PasswordHash, password) == PasswordVerificationResult.Success) 
            {
                // At this point the user should be logged, which means I am going to need some auth utils for JWT and stored Auth tokens either in in-memory cache, redis or postgres
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"Succesful login attempt as user {loginUser.UserName}",
                    Data = loginUser
                };
            }

            return new ServiceResult
            {
                IsSuccess = false,
                Message = "Invalid password"
            };
        }

        public Task<ServiceResult> DeleteUser(User user)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserById(Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
