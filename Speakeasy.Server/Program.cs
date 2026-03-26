using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Speakeasy.Server.Controllers.ApiVersion1.Hubs;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Database.Repositories;
using Speakeasy.Server.Models.Options;

namespace Speakeasy.Server;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigureServices(builder.Services, builder.Configuration);
            
            var app = builder.Build();
            ConfigureApplication(app);

            await app.RunAsync();
        }
        catch (Exception e)
        {
            Log.Error(e, "An uncaught exception has occured. Speakeasy is closing...");
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
        return 0;
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration config)
    {
        services.AddSerilog((sp, lc) => lc
            .ReadFrom.Configuration(config)
            .ReadFrom.Services(sp)
            .Enrich.FromLogContext()
            .WriteTo.Console());

        services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                };
            });

        var connectionStringOptions = config.GetSection("ConnectionStrings").Get<ConnectionStringOptions>();
        ArgumentNullException.ThrowIfNull(connectionStringOptions);

        // Add database services
        services.AddSingleton(connectionStringOptions);
        services.AddDbContext<SpeakeasyDbContext>(builder =>
            SpeakeasyDbContextOptionsConfigurator.Configure(builder, connectionStringOptions));
        services.AddDbContextFactory<SpeakeasyDbContext>(builder =>
            SpeakeasyDbContextOptionsConfigurator.Configure(builder, connectionStringOptions));
        
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Add ASP.NET Core Identity services
        services.AddAuthorization();
        services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<SpeakeasyDbContext>();

        services.AddSignalR();
    }

    private static void ConfigureApplication(WebApplication app)
    {
        // Use attribute base routing
        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();
        app.MapIdentityApi<User>();
        app.MapHub<SpeakeasyV1Hub>("/hub/v1");
    }
}