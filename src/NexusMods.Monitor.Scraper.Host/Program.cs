using AutoMapper;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application;
using NexusMods.Monitor.Scraper.Application.BackgroundServices;
using NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments;
using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Scraper.Application.Options;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.CommentAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsGameAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsThreadAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Scraper.Infrastructure.Contexts;
using NexusMods.Monitor.Scraper.Infrastructure.Models.Comments;
using NexusMods.Monitor.Scraper.Infrastructure.Models.Issues;
using NexusMods.Monitor.Scraper.Infrastructure.Repositories;
using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Host.Extensions;

using NodaTime;

using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

using System;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host
{
    public static class Program
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
                Log.Warning("Starting.");

                var hostBuilder = CreateHostBuilder(args);

                var host = hostBuilder.Build();
                await EnsureDatabasesCreated(host);
                host.MigrateDbContext<NexusModsDb>((context, services) =>
                {
                    var logger = services.GetRequiredService<ILogger<NexusModsDbSeed>>();
                    new NexusModsDbSeed().SeedAsync(logger, context).Wait();
                });

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal exception.");
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
                services.AddAutoMapper(cfg =>
                {
                    cfg.CreateMap<Instant, DateTimeOffset>().ConvertUsing(i => i.ToDateTimeOffset());

                    cfg.CreateMap<CommentEntity, CommentDTO>();
                    cfg.CreateMap<CommentReplyEntity, CommentDTO.CommentReplyDTO>();

                    cfg.CreateMap<IssueEntity, IssueDTO>();
                    cfg.CreateMap<IssueStatusEnumeration, IssueDTO.IssueStatusDTO>();
                    cfg.CreateMap<IssuePriorityEnumeration, IssueDTO.IssuePriorityDTO>();
                    cfg.CreateMap<IssueContentEntity, IssueDTO.IssueContentDTO>();
                    cfg.CreateMap<IssueReplyEntity, IssueDTO.IssueReplyDTO>();
                });
                services.AddMediatR(typeof(CommentAddCommandHandler).Assembly);
                services.AddMemoryCache();
                services.AddHttpClient();
                services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);
                services.AddEventBusNatsAndEventHandlers(context.Configuration.GetSection("EventBus"), typeof(NexusModsOptions).Assembly);

                services.Configure<NexusModsOptions>(context.Configuration.GetSection("NexusMods"));
                services.Configure<SubscriptionsOptions>(context.Configuration.GetSection("Subscriptions"));

                services.AddDbContext<NexusModsDb>(opt => opt.UseNpgsql(context.Configuration.GetConnectionString("NexusMods"), o => o.UseNodaTime()));

                services.AddHostedService<NexusModsIssueMonitor>();
                services.AddHostedService<NexusModsCommentsMonitor>();

                services.AddTransient<ICommentRepository, CommentRepository>();
                services.AddTransient<IIssueRepository, IssueRepository>();
                services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();

                services.AddTransient<INexusModsIssuesRepository, NexusModsIssuesRepository>();
                services.AddTransient<INexusModsCommentsRepository, NexusModsCommentsRepository>();
                services.AddTransient<INexusModsGameRepository, NexusModsGameRepository>();
                services.AddTransient<INexusModsThreadRepository, CachedNexusModsThreadRepository>();
                //services.AddTransient<INexusModsThreadRepository, NexusModsThreadRepository>();
            })
            .ConfigureAppConfiguration(config => config.AddEnvironmentVariables())
            .UseSerilog();
    }
}