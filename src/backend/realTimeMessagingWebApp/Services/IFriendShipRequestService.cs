using realTimeMessagingWebApp.Services.ResponseModels;

namespace realTimeMessagingWebApp.Services
{
    public interface IFriendShipRequestService
    {
        public Task<ServiceResult> MakeFriendShipRequest(Guid initiator, Guid recipient);

        public Task<ServiceResult> AcceptFriendShipRequest(Guid acceptor, Guid friendShipId);

        public Task<ServiceResult> DeclineFriendShipRequest(Guid decliner, Guid friendShipId);
    }
}
