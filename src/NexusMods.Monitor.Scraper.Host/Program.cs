using BetterHostedServices;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments;
using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Scraper.Application.Queries.Comments;
using NexusMods.Monitor.Scraper.Application.Queries.Issues;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsGames;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsThreads;
using NexusMods.Monitor.Scraper.Application.Queries.Subscriptions;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Scraper.Host.BackgroundServices;
using NexusMods.Monitor.Scraper.Host.Options;
using NexusMods.Monitor.Scraper.Infrastructure.Contexts;
using NexusMods.Monitor.Scraper.Infrastructure.Repositories;
using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Domain.SeedWork;
using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Shared.Host.Extensions;

using NodaTime;

using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

using System;
using System.Reflection;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = GetInitialConfiguration();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Warning("Starting");

                var hostBuilder = CreateHostBuilder(args);

                var host = hostBuilder.Build();
                await EnsureDatabasesCreated(host);
                host.MigrateDbContext<NexusModsDb>((context, services) =>
                {
                    var logger = services.GetRequiredService<ILogger<NexusModsDbSeed>>();
                    NexusModsDbSeed.SeedAsync(logger, context).Wait();
                });

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal exception");
                throw;
            }
            finally
            {
                Log.Warning("Stopped.");
                Log.CloseAndFlush();
            }
        }

        private static IConfigurationRoot GetInitialConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static async Task EnsureDatabasesCreated(IHost host)
        {
            using var scope = host.Services.CreateScope();
            using var nexusModsDb = scope.ServiceProvider.GetRequiredService<NexusModsDb>();
            //await nexusModsDb.Database.EnsureDeletedAsync();
            await nexusModsDb.Database.EnsureCreatedAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddApplication();

                services.AddAutoMapper(cfg =>
                {
                    cfg.CreateMap<Instant, DateTimeOffset>().ConvertUsing(i => i.ToDateTimeOffset());

                    cfg.CreateMap<CommentEntity, CommentDTO>();
                    cfg.CreateMap<CommentReplyEntity, CommentReplyDTO>();

                    cfg.CreateMap<IssueEntity, IssueDTO>();
                    cfg.CreateMap<IssueStatusEnumeration, IssueStatusDTO>();
                    cfg.CreateMap<IssuePriorityEnumeration, IssuePriorityDTO>();
                    cfg.CreateMap<IssueContentEntity, IssueContentDTO>();
                    cfg.CreateMap<IssueReplyEntity, IssueReplyDTO>();
                });
                services.AddMediatR(typeof(CommentAddCommandHandler).Assembly);

                services.AddHttpClient("Metadata.API", (sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<MetadataAPIOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.APIEndpointV1);
                }).AddPolicyHandler(PollyUtils.PolicySelector);
                services.AddHttpClient("Subscriptions.API", (sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<SubscriptionsAPIOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.APIEndpointV1);
                }).AddPolicyHandler(PollyUtils.PolicySelector);

                services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);
                services.AddEventBusNatsAndEventHandlers(context.Configuration.GetSection("EventBus"), typeof(HostExtensions).Assembly);

                services.AddBetterHostedServices();

                services.Configure<MetadataAPIOptions>(context.Configuration.GetSection("MetadataAPI"));
                services.Configure<SubscriptionsAPIOptions>(context.Configuration.GetSection("SubscriptionsAPI"));

                services.AddDbContext<NexusModsDb>(opt => opt.UseNpgsql(context.Configuration.GetConnectionString("NexusMods"), o => o.UseNodaTime()));
                services.AddTransient<IUnitOfWork>(sp => sp.GetRequiredService<NexusModsDb>());

                services.AddHostedServiceAsSingleton<NexusModsIssueMonitor>();
                services.AddHostedServiceAsSingleton<NexusModsCommentsMonitor>();

                services.AddTransient<ICommentRepository, CommentRepository>();
                services.AddTransient<IIssueRepository, IssueRepository>();

                services.AddTransient<INexusModsCommentQueries, NexusModsCommentQueries>();
                services.AddTransient<INexusModsGameQueries, NexusModsGameQueries>();
                services.AddTransient<INexusModsIssueQueries, NexusModsIssueQueries>();
                services.AddTransient<INexusModsThreadQueries, NexusModsThreadQueries>();
                services.AddTransient<ICommentQueries, CommentQueries>();
                services.AddTransient<IIssueQueries, IssueQueries>();
                services.AddTransient<ISubscriptionQueries, SubscriptionQueries>();
            })
            .ConfigureAppConfiguration(config => config.AddEnvironmentVariables())
            .UseSerilog();
    }
}