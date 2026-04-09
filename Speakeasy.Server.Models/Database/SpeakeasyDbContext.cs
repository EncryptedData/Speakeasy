using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Speakeasy.Server.Models.Database;

public class SpeakeasyDbContext : IdentityDbContext<User>
{
    public SpeakeasyDbContext(DbContextOptions<SpeakeasyDbContext> options) :
        base(options)
    {
    }
    
    public DbSet<Group> Groups { get; set; }
    
    public DbSet<GroupRole> GroupRoles { get; set; }
    
    public DbSet<Channel> Channels { get; set; }
    
    public DbSet<Message> Messages { get; set; }
    
    public DbSet<StoredFile> Files { get; set; }
    
    public DbSet<CustomEmoji> CustomEmojis { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Must be called first because the identity library
        base.OnModelCreating(modelBuilder);
        
        // Force a many to many relationship, otherwise EF Core only thinks
        // there's going to be one reaction per user globally
        modelBuilder.Entity<MessageReaction>()
            .HasMany<User>(e => e.Reactors)
            .WithMany();
    }
}