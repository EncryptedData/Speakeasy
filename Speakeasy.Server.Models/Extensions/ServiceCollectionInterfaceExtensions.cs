using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Options;

namespace Speakeasy.Server.Models.Extensions;

public static class ServiceCollectionInterfaceExtensions
{
    public static IServiceCollection AddModelServices(this IServiceCollection services, IConfiguration config)
    {
        var connectionStringOptions = config.GetSection("ConnectionStrings").Get<ConnectionStringOptions>();
        ArgumentNullException.ThrowIfNull(connectionStringOptions);

        services.AddSingleton(connectionStringOptions);
        services.AddDbContext<SpeakeasyDbContext>(builder =>
            SpeakeasyDbContextOptionsConfigurator.Configure(builder, connectionStringOptions));
        services.AddDbContextFactory<SpeakeasyDbContext>(builder =>
            SpeakeasyDbContextOptionsConfigurator.Configure(builder, connectionStringOptions));

        return services;
    }
}