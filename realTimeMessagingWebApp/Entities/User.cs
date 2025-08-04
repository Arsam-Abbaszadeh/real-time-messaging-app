namespace realTimeMessagingWebApp.Entities
{
    public class User
    {
        public Guid userID { get; set; }
        public string userName { get; set; }
        public string passwordHash { get; set; }
        
        public ICollection<GroupChat> GroupChats { get; set; } = new HashSet<GroupChat>();
    }
}
