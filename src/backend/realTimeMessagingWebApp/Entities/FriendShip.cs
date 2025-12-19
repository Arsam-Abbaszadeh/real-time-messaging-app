using realTimeMessagingWebApp.Enums;

namespace realTimeMessagingWebApp.Entities
{
    public class FriendShip
    {
        public Guid FriendShipId { get; set; } // primary key
        
        public Guid UserAId { get; set; } 
        public Guid UserBId { get; set; }

        public Guid FriendShipInitiator { get; set; } // the user who sent the friend request

        public DateTime FriendShipCreationDate { get; set; }

        public FriendShipStatus Status { get; set; }

        // nav properties
        public User UserA { get; set; }

        public User UserB { get; set; }
    }
}
