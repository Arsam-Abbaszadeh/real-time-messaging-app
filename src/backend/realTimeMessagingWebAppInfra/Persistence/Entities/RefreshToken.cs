namespace realTimeMessagingWebAppInfra.Persistence.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpirationUtc { get; set; }
        public bool isValid { get; set; } = true; // idk if this is how you do default vals in ef core
        // nav prop
        public User User { get; set; }
    }
}
