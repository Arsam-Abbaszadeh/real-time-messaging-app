namespace realTimeMessagingWebApp.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }

        public DateTime SignUpDate { get; set; }
        
        public ICollection<GroupChat> GroupChats { get; set; } = new HashSet<GroupChat>();
        
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();

        public override bool Equals(object? obj)
        {
            if (obj is not User)
            {
                return false;
            }

            var other = (User)obj;
            return UserId == other.UserId;
        }
    }
}
