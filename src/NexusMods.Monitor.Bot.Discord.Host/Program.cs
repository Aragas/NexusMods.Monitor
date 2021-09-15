using BetterHostedServices;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NexusMods.Monitor.Bot.Discord.Application.CommandHandlers;
using NexusMods.Monitor.Bot.Discord.Application.IntegrationEventHandlers.Comments;
using NexusMods.Monitor.Bot.Discord.Application.Queries.Authorizations;
using NexusMods.Monitor.Bot.Discord.Application.Queries.RateLimits;
using NexusMods.Monitor.Bot.Discord.Application.Queries.Subscriptions;
using NexusMods.Monitor.Bot.Discord.Host.BackgroundServices;
using NexusMods.Monitor.Bot.Discord.Host.Options;
using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Shared.Host.Extensions;

using NodaTime;

using Serilog;

using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Host
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
            .AddDiscord()
            .ConfigureAppConfiguration((hostingContext, config) => config.AddEnvironmentVariables())
            .UseSerilog();

        private static IHostBuilder AddDiscord(this IHostBuilder builder) => builder.ConfigureServices((context, services) =>
        {
            services.AddOptions<DiscordOptions>()
                .Bind(context.Configuration.GetSection("Discord"))
                .ValidateViaFluent<DiscordOptions, DiscordOptionsValidator>()
                .ValidateOnStart();

            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<IDiscordClient, DiscordSocketClient>(sp => sp.GetRequiredService<DiscordSocketClient>());
            services.AddSingleton<CommandService>();

            services.AddHostedServiceAsSingleton<DiscordService>();
        });
    }
}