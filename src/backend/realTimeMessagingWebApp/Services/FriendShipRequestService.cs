using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebAppData.Entities;
using realTimeMessagingWebAppData.Enums;
using realTimeMessagingWebAppData;
using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public class FriendShipRequestService(Context dbContext) : IFriendShipRequestService
    {
        readonly Context _context = dbContext;

        public async Task<ServiceResult> AcceptFriendShipRequest(Guid acceptor, Guid friendShipId) // consider allowing for declind friend requests to make a new one
        {
            var result = await ReplyToFriendShipRequest(FriendShipStatus.Friends, acceptor, friendShipId);

            if (result)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = "Friend ship succesfully accepted"
                };
            }

            return new ServiceResult
            {
                IsSuccess = false,
                Message = "Pending friend request does not exist"
            };
        }

        public async Task<ServiceResult> DeclineFriendShipRequest(Guid decliner, Guid friendShipId)
        {
            var result = await ReplyToFriendShipRequest(FriendShipStatus.NotFriends, decliner, friendShipId);

            if (result)
            {
                return new ServiceResult
                {
                    IsSuccess = true,
                    Message = "Friend ship succesfully declined"
                };
            }

            return new ServiceResult
            {
                IsSuccess = false,
                Message = "Pending friend request does not exist"
            };
        }

        async Task<bool> ReplyToFriendShipRequest(FriendShipStatus statusChange, Guid actionTaker, Guid friendShipId)
        {
            var existingRequest = await _context.FriendShips.FirstOrDefaultAsync(fr =>
            fr.FriendShipId == friendShipId &&
            (fr.UserAId == actionTaker || fr.UserBId == actionTaker) &&
            fr.FriendShipInitiator != actionTaker);

            var status = existingRequest?.Status;

            if (status.HasValue && status.Value == FriendShipStatus.PendingRequest)
            {
                existingRequest!.Status = statusChange;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<ServiceResult> MakeFriendShipRequest(Guid initiator, Guid recipient)
        {
            var existingRequest = (await _context.FriendShips.FirstOrDefaultAsync(fr =>
            fr.UserAId == initiator && fr.UserBId == recipient ||
            fr.UserBId == initiator && fr.UserAId == recipient))?.Status;

            if (existingRequest.HasValue) // exsiting request which is bad
            {
                if (existingRequest == FriendShipStatus.Friends)
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = "Users are already friends" // could probs be a better error message
                    };
                }
                else if (existingRequest == FriendShipStatus.PendingRequest)
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = "There is already a pending friend request between these users"
                    };
                }
                else
                {
                    return new ServiceResult
                    {
                        IsSuccess = false,
                        Message = "There is already a declined friend request between these users"
                    };
                }
            }

            var reqeust = new FriendShip
            {
                FriendShipId = Guid.NewGuid(),
                UserAId = initiator,
                UserBId = recipient,
                FriendShipInitiator = initiator,
                FriendShipCreationDate = DateTime.UtcNow,
                Status = FriendShipStatus.PendingRequest
            };

            await _context.AddAsync(reqeust);
            await _context.SaveChangesAsync();

            return new ServiceResult
            {
                IsSuccess = true,
                Message = "Friendship requested successfully"
            };
        }
    }
}
