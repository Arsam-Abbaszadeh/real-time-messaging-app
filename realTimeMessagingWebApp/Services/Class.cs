using realTimeMessagingWebApp.Auth;
using realTimeMessagingWebApp.Data;
using realTimeMessagingWebApp.Entities;
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
