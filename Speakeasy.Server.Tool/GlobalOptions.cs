using System.CommandLine;

namespace Speakeasy.Server.Tool;

public static class GlobalOptions
{
    public static Option<string> DatabaseConnectionStringOption = new("-s", "--db-conn")
    {
        Description = "The connection string to the database. If not provided, the value from the appsettings.json's will be used.",
        DefaultValueFactory = _ => null!,
    };
    
    public static Option<bool> DryRunOption = new("-d", "--dry-run")
    {
        Description = "If set, the tool will simulate the actions without making any changes.",
        DefaultValueFactory = _ => false,
    };
}