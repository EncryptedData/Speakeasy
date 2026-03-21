using Microsoft.EntityFrameworkCore;

namespace Speakeasy.Server.Models.Database;

public class SpeakeasyDbContext : DbContext
{
    public SpeakeasyDbContext(DbContextOptions<SpeakeasyDbContext> options) :
        base(options)
    {
    }
}