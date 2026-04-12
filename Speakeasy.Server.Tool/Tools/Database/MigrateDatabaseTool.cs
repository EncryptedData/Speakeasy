using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Speakeasy.Server.Common.Extensions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Options;

namespace Speakeasy.Server.Tool.Tools.Database;

public static class MigrateDatabaseTool
{
    public static async Task<int> RunAsync(string? connectionString, bool dryRun)
    {
        if (connectionString.IsNullOrEmpty())
        {
            ConfigurationBuilder configurationBuilder = new();
            configurationBuilder.Add(new JsonConfigurationSource()
                {
                    Path = "appsettings.json",
                    Optional = false,
                    ReloadOnChange = false,
                }).Add(new JsonConfigurationSource()
                {
                    Path = "appsettings.Development.json",
                    Optional = true,
                    ReloadOnChange = false,
                })
                .Add(new JsonConfigurationSource()
                {
                    Path = "appsettings.local.json",
                    Optional = true,
                    ReloadOnChange = false,
                });
            
            IConfiguration config = configurationBuilder.Build();
            connectionString = config.GetConnectionString("Database");
        }
        
        var dbOptionsBuilder = new DbContextOptionsBuilder<SpeakeasyDbContext>();
        SpeakeasyDbContextOptionsConfigurator.Configure(dbOptionsBuilder, new ConnectionStringOptions()
        {
            Database = connectionString!,
        });

        var dbContext = new SpeakeasyDbContext(dbOptionsBuilder.Options);

        var pendingMigrations = (await dbContext.Database.GetPendingMigrationsAsync()).ToList();
        Console.WriteLine($"Found {pendingMigrations.Count} pending migrations.");
        
        if (!dryRun && pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migration completed successfully.");
        }
        
        return 0;
    }
}