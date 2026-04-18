using System.IO.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Speakeasy.Server.Common.Exceptions;
using Speakeasy.Server.Controllers.ApiVersion1.Hubs;
using Speakeasy.Server.Conventions;
using Speakeasy.Server.Models.Abstractions;
using Speakeasy.Server.Models.Converters;
using Speakeasy.Server.Models.Database;
using Speakeasy.Server.Models.Database.Repositories;
using Speakeasy.Server.Models.Options;
using Speakeasy.Server.Models.Services;
using Speakeasy.Server.Models.Transmission;
using Speakeasy.Server.Storage;
using Speakeasy.Server.Storage.Abstractions;
using Speakeasy.Server.Storage.Options;
using Speakeasy.Server.Storage.Validators;

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

        services.AddControllers(options => options.Conventions.Add(new InferSuccessCodeResponseConvention()))
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy(),
                };

                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

        services.AddHttpContextAccessor();

        var allowedOrigins = config.GetSection("Server:AllowedCorsOrigins").Get<string[]>() ?? ["*"];
        ExceptionUtil.ThrowIfFalse<Exception>(allowedOrigins.Any());

        services.AddCors(options => options.AddDefaultPolicy(policy => policy.WithOrigins(allowedOrigins)));

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
        
        // Add Model converters
        services.AddSingleton<IModelConverter<Group, GroupDto>, GroupModelConverter>();
        services.AddSingleton<IModelConverter<GroupRole, GroupRoleDto>, GroupRoleModelConverter>();
        services.AddSingleton<IModelConverter<CustomEmoji, CustomEmojiDto>, CustomEmojiModelConverter>();
        services.AddSingleton<IModelConverter<Gif, GifDto>, GifModelConverter>();
        
        // These need to be scoped because it accesses the HttpContext which can't be a singleton
        services.AddScoped<IModelConverter<Channel, ChannelDto>, ChannelModelConverter>();
        services.AddScoped<IModelConverter<Message, MessageDto>, MessageModelConverter>();
        
        // Add ASP.NET Core Identity services
        services.AddAuthorization();
        services.AddIdentityApiEndpoints<User>()
            .AddEntityFrameworkStores<SpeakeasyDbContext>();

        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();

        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                var components = document.Components ?? new OpenApiComponents();
                document.Components = components;
                components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
                components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Description = "Enter your bearer token from /login"
                });

                // Build a set of (path, HTTP method) pairs that require auth, so we can apply
                // per-operation security. Must be done here (not an operation transformer) so
                // we can pass `document` to OpenApiSecuritySchemeReference for correct serialization.
                var authorizedEndpoints = context.DescriptionGroups
                    .SelectMany(g => g.Items)
                    .Where(d => d.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>().Any()
                             && !d.ActionDescriptor.EndpointMetadata.OfType<IAllowAnonymous>().Any())
                    .Select(d => ($"/{d.RelativePath?.TrimStart('/')}", d.HttpMethod?.ToUpperInvariant()))
                    .ToHashSet();

                var bearerRequirement = new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("Bearer", document)] = []
                };

                foreach (var (path, pathItem) in document.Paths)
                {
                    foreach (var (method, operation) in pathItem.Operations ?? [])
                    {
                        if (authorizedEndpoints.Contains((path, method.Method.ToUpperInvariant())))
                            operation.Security = [bearerRequirement];
                    }
                }

                return Task.CompletedTask;
            });
        });

        services.AddSignalR();
        services.AddSingleton<ISpeakeasyV1HubService, SpeakeasyV1HubService>();

        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddKeyedSingleton<IFileStore, LocalFileStore>(LocalFileStore.Key);
        services.AddSingleton<IFileStore>(sp => sp.GetRequiredKeyedService<IFileStore>(LocalFileStore.Key));
        services.AddSingleton<ITemporaryFileStore, TemporaryFileStore>();
        services.AddSingleton<IImageValidator, ImageFileValidator>();
        services.AddSingleton<IFileValidator<ImageValidationProperties>>(sp =>
            sp.GetRequiredService<IImageValidator>());
        services.AddSingleton<StorageOptions>(sp =>
        {
            var configuration = sp.GetRequiredService<IConfiguration>();

            return configuration.GetSection("Storage").Get<StorageOptions>()!;
        });
    }

    private static void ConfigureApplication(WebApplication app)
    {
        // Use attribute base routing
        app.UseRouting();
        app.UseCors();
        app.UseAuthorization();
        app.MapControllers();
        app.MapGroup("api/v1/auth")
            .MapIdentityApi<User>();
        app.MapHub<SpeakeasyV1Hub>("/hub/v1");

        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("local"))
        {
            // Map default endpoint at /openapi/v1.json
            app.MapOpenApi();
        }
    }
}