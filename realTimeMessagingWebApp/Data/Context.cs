using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebApp.Entities;

namespace realTimeMessagingWebApp.Data
{
    public class Context(DbContextOptions<Context> options) : DbContext(options)
    {
        //public Context(DbContextOptions<Context> options) : base(options) { } // instead using the primary constructor, pretty neat syntax

        public DbSet<User> Users { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupChatConnector> GroupChatConnectors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.GroupChats)
                .WithMany(gc => gc.GroupChatMembers)
                .UsingEntity<GroupChatConnector>();
        }

    }
}
