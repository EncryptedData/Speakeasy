using System.CommandLine;
using Speakeasy.Server.Tool.Tools.Database;

namespace Speakeasy.Server.Tool;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        RootCommand rootCmd = new();
        DatabaseOptions.RegisterDatabaseOptions(rootCmd);

        var result = rootCmd.Parse(args);
        return await result.InvokeAsync();
    }

}