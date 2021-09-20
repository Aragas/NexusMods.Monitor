using BetterHostedServices;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
using NexusMods.Monitor.Shared.Application.Models;
using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Shared.Host.Extensions;

using NodaTime;

using System;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host
{
    public class Program
    {
        public static async Task Main(string[] args) => await new HostManager(CreateHostBuilder).StartAsync(args);

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
            .AddSubscriptionsHttpClient();
    }
}