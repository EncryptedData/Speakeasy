using System.CommandLine;

namespace Speakeasy.Server.Tool.Tools.Database;

public static class DatabaseOptions
{
    public static void RegisterDatabaseOptions(RootCommand rootCmd)
    {
        var databaseCmd = new Command("database", "Database management commands");
        rootCmd.Add(databaseCmd);
        
        var migrateCmd = new Command("migrate", "Apply pending database migrations")
        {
            GlobalOptions.DatabaseConnectionStringOption,
            GlobalOptions.DryRunOption
        };
        migrateCmd.SetAction(pr =>
            MigrateDatabaseTool.RunAsync(
                pr.GetValue(GlobalOptions.DatabaseConnectionStringOption),
                pr.GetValue(GlobalOptions.DryRunOption)));
        databaseCmd.Add(migrateCmd);
    }
}