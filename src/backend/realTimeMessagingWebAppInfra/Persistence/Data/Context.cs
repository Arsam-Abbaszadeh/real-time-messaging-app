using Microsoft.EntityFrameworkCore;
using realTimeMessagingWebAppInfra.Persistence.Entities;

namespace realTimeMessagingWebAppInfra.Persistence.Data;

public class Context(DbContextOptions<Context> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<GroupChat> GroupChats { get; set; }
    public DbSet<GroupChatConnector> GroupChatConnectors { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<FriendShip> FriendShips { get; set; }
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // --- DB-generated UUIDs (Postgres) ---
        const string uuidGenSql = "gen_random_uuid()";

        modelBuilder.Entity<User>()
            .Property(e => e.UserId)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql(uuidGenSql);

        modelBuilder.Entity<GroupChat>()
            .Property(e => e.GroupChatId)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql(uuidGenSql);

        modelBuilder.Entity<GroupChatConnector>()
            .Property(e => e.GroupChatConnectorId)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql(uuidGenSql);

        modelBuilder.Entity<FriendShip>()
            .Property(e => e.FriendShipId)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql(uuidGenSql);

        modelBuilder.Entity<RefreshToken>()
            .Property(e => e.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql(uuidGenSql);

        modelBuilder.Entity<Message>()
            .Property(e => e.MessageId)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql(uuidGenSql);

        modelBuilder.Entity<MessageAttachment>()
            .Property(e => e.MessageAttachmentId)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql(uuidGenSql);

        // --- Relationships ---
        modelBuilder.Entity<User>()
            .HasMany(u => u.GroupChats)
            .WithMany(gc => gc.GroupChatMembers)
            .UsingEntity<GroupChatConnector>();

        modelBuilder.Entity<Message>()
            .HasMany(m => m.Attachments)
            .WithOne(ma => ma.Message)
            .HasForeignKey(ma => ma.MessageId);
    }
}
