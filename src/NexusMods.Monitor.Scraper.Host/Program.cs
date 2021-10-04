using BetterHostedServices;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application.CommandHandlers.Comments;
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
using NexusMods.Monitor.Scraper.Infrastructure.Contexts;
using NexusMods.Monitor.Scraper.Infrastructure.Repositories;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Shared.Host.Extensions;
using NexusMods.Monitor.Shared.Infrastructure.Extensions;

using NodaTime;

using Polly;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host
{
    public class Program
    {
        public static async Task Main(string[] args) => await new HostManager(CreateHostBuilder)
            .ExecuteBeforeRun(async host =>
            {
                await EnsureDatabasesCreated(host);
            })
            .StartAsync(args);

        private static async Task EnsureDatabasesCreated(IHost host)
        {
            var retryPolicy = Policy.Handle<Exception>(ex => ex.GetType() != typeof(TaskCanceledException))
                .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(2),
                    (ex, time) =>
                    {
                        using var scope = host.Services.CreateScope();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Exception during PostgreSQL connection. Waiting {time}...", time);
                    });

            await retryPolicy.ExecuteAsync(async token =>
            {
                using var scope = host.Services.CreateScope();
                using var nexusModsDb = scope.ServiceProvider.GetRequiredService<NexusModsDb>();
                await nexusModsDb.EnsureTablesCreatedAsync(token);
            }, CancellationToken.None);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddApplication();

                services.AddMediatR(typeof(CommentAddCommandHandler).Assembly);

                services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);

                services.AddBetterHostedServices();

                services.AddDbContext<NexusModsDb>(opt => opt.UseNpgsql(context.Configuration.GetConnectionString("NexusMods"), o => o.UseNodaTime()));

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
            .AddEventBusNatsAndEventHandlers()
            .AddMetadataHttpClient()
            .AddSubscriptionsHttpClient()
            //.AddNpgsqlConnection<NexusModsDb>("NexusMods")
        ;
    }
}