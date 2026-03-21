using Microsoft.EntityFrameworkCore;
using Speakeasy.Server.Models.Options;

namespace Speakeasy.Server.Models.Database;

public static class SpeakeasyDbContextOptionsConfigurator
{
    public static void Configure(DbContextOptionsBuilder contextOptions, ConnectionStringOptions options)
    {
        contextOptions.UseNpgsql(options.Database);
    }
}