using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Speakeasy.Server.Models.Options;

namespace Speakeasy.Server.Models.Database.Configuration;

public class SpeakeasyDesignTimeDbContextFactory : IDesignTimeDbContextFactory<SpeakeasyDbContext>
{
    public SpeakeasyDbContext CreateDbContext(string[] args)
    {
        DbContextOptionsBuilder<SpeakeasyDbContext> optionsBuilder = new();
        SpeakeasyDbContextOptionsConfigurator.Configure(optionsBuilder, new ConnectionStringOptions()
        {
            Database = "",
        });

        return new SpeakeasyDbContext(optionsBuilder.Options);
    }
}