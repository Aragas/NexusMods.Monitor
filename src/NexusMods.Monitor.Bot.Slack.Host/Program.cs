using BetterHostedServices;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NexusMods.Monitor.Bot.Slack.Application.CommandHandlers;
using NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Comments;
using NexusMods.Monitor.Bot.Slack.Application.Queries.Authorizations;
using NexusMods.Monitor.Bot.Slack.Application.Queries.RateLimits;
using NexusMods.Monitor.Bot.Slack.Application.Queries.Subscriptions;
using NexusMods.Monitor.Bot.Slack.Host.BackgroundServices;
using NexusMods.Monitor.Bot.Slack.Host.Options;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Shared.Host.Extensions;

using NodaTime;

using SlackNet.Bot;

using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Host
{
    public static class Program
    {
        public static async Task Main(string[] args) => await new HostManager(CreateHostBuilder).StartAsync(args);

        public static IHostBuilder CreateHostBuilder(string[] args) => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddApplication();

                services.AddMediatR(typeof(SubscribeCommandHandler).Assembly);

                services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);

                services.AddBetterHostedServices();

                services.AddTransient<ISubscriptionQueries, SubscriptionQueries>();
                services.AddTransient<IRateLimitQueries, RateLimitQueries>();
                services.AddTransient<IAuthorizationQueries, AuthorizationQueries>();
            })
            .AddEventBusNatsAndEventHandlers(typeof(CommentAddedNewIntegrationEventHandler).Assembly)
            .AddSubscriptionsHttpClient()
            .AddMetadataHttpClient()
            .AddSlack();


        private static IHostBuilder AddSlack(this IHostBuilder builder) => builder.ConfigureServices((context, services) =>
        {
            services.AddValidatedOptions<SlackOptions, SlackOptionsValidator>(context.Configuration.GetSection("Slack"));

            services.AddSingleton<ISlackBot, SlackBotWrapper>();

            services.AddHostedServiceAsSingleton<SlackService>();
        });
    }
}