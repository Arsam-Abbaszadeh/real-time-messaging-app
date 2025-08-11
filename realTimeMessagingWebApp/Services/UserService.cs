using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebApp.Auth;
using realTimeMessagingWebApp.Data;
using realTimeMessagingWebApp.Entities;
using realTimeMessagingWebApp.Services.ResponseModels;
using System.Reflection;

namespace realTimeMessagingWebApp.Services
{
    public class UserService(Context context) : IUserService
    {
        private readonly Context _context = context;

        public Task<ServiceResult> CreateNewUser(User user)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResult> CreateNewUser(User user, string password)
        {
            if (user is null) throw new ArgumentNullException(nameof(user)); // should never really happen
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

            _context.Add(user);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = "New User Saved to Db"
            };   
        }

        public async Task<ServiceResult> LoginUser(string userName, string password)
        {
            if (userName is null) throw new ArgumentNullException(nameof(userName)); // should never really happen and a bit random
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
                // TODO implement auth services

                // For now just return success service
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = $"Succesful login attempt as user {loginUser.UserName}"
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
