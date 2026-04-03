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
    
    public DbSet<Channel> Channels { get; set; }
    
    public DbSet<Message> Messages { get; set; }
    
    public DbSet<StoredFile> Files { get; set; }
}