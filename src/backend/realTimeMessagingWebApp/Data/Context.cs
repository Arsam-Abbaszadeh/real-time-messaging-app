using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebApp.Entities;

namespace realTimeMessagingWebApp.Data
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {

        public DbSet<User> Users { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupChatConnector> GroupChatConnectors { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<FriendShip> FriendShips { get; set; }
        
        public DbSet<Message> Messages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.GroupChats)
                .WithMany(gc => gc.GroupChatMembers)
                .UsingEntity<GroupChatConnector>();

        }

    }
}
