using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebAppData;
using realTimeMessagingWebAppData.Enums;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services;

public class RelationShipService(Context dbContext) // dont really know if this needs to be seperate service in gc service but whatever
{
    readonly Context _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<ServiceResult> AreFriends(Guid userAId, Guid userBId)
    {
        var result = await _context.FriendShips.AnyAsync(f =>
                (f.UserAId == userAId && f.UserBId == userBId) ||
                (f.UserAId == userBId && f.UserBId == userAId) &&
                f.Status ==  FriendShipStatus.Friends); // is this actually more practical?

        if (result)
        {
            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with Id {userAId} is friends with User with id {userBId}"
            };
        }
        else
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Message = $"User with Id {userAId} is not friends with User with id {userBId}"
            };
        }
    }

    public async Task<ServiceResult> AreAllFriendsOfUser(Guid userId, ICollection<Guid> friendIds)
    {
        var count = await _context.FriendShips.Where(f =>
                (f.UserAId == userId && friendIds.Contains(f.UserBId)) ||
                (f.UserBId == userId && friendIds.Contains(f.UserAId)) &&
                f.Status == FriendShipStatus.Friends)
                .Select(f => f.FriendShipId)
                .Distinct()
                .CountAsync();

        if (count == friendIds.Count)
        {
            return new ServiceResult
            {
                IsSuccess = true,
                Message = $"User with Id {userId} is friends with all specified users"
            };
        }
        else
        {
            return new ServiceResult
            {
                IsSuccess = false,
                Message = $"User with Id {userId} is not friends with all specified users"
            };
        }
    }
}
