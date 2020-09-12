using Enbiso.NLib.EventBus;

using MediatR;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Slack.Application;
using NexusMods.Monitor.Bot.Slack.Application.Commands;
using NexusMods.Monitor.Bot.Slack.Application.Queries;

using NodaTime;
using NodaTime.Extensions;

using SlackNet.Bot;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Host.BackgroundServices
{
    /// <summary>
    /// Manages the Discord connection.
    /// </summary>
    public sealed class SlackService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly SlackBot _bot;
        private readonly IClock _clock;
        private readonly IMediator _mediator;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly IEventSubscriber _eventSubscriber;

        public SlackService(ILogger<SlackService> logger,
            SlackBot bot,
            IClock clock,
            IMediator mediator,
            ISubscriptionQueries subscriptionQueries,
            IEventSubscriber eventSubscriber)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _bot = bot ?? throw new ArgumentNullException(nameof(bot));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _eventSubscriber = eventSubscriber ?? throw new ArgumentNullException(nameof(eventSubscriber));
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _bot.OnMessage += Bot_OnMessageReceived;

            await _bot.Connect(cancellationToken);

            await _eventSubscriber.Subscribe();

            _logger.LogWarning("Started Slack Bot.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _bot.OnMessage -= Bot_OnMessageReceived;
            _bot.Dispose();
            _logger.LogWarning("Stopped Slack Bot.");

            _eventSubscriber.Dispose();

            return Task.CompletedTask;
        }

        private async void Bot_OnMessageReceived(object? sender, IMessage message)
        {
            if (!message.Conversation.IsChannel)
                return;

            const string prefix = "!nmm ";
            if (message.Text.StartsWith(prefix))
            {
                var command = message.Text.Remove(0, prefix.Length);

                const string subscribe = "subscribe ";
                if (command.StartsWith("subscribe "))
                {
                    var argsText = command.Remove(0, subscribe.Length);
                    var args = argsText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (args.Length == 2 && uint.TryParse(args[0], out var gameId) && uint.TryParse(args[1], out var modId))
                    {
                        if (await _mediator.Send(new SubscribeCommand(message.Conversation.Id, gameId, modId)))
                        {
                            await message.ReplyWith("Successful!");
                            return;
                        }
                    }
                    else
                    {
                        await message.ReplyWith("Failed!");
                        return;
                    }
                }

                const string unsubscribe = "unsubscribe ";
                if (command.StartsWith(unsubscribe))
                {
                    var argsText = command.Remove(0, unsubscribe.Length);
                    var args = argsText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (args.Length == 2 && uint.TryParse(args[0], out var gameId) && uint.TryParse(args[1], out var modId))
                    {
                        if (await _mediator.Send(new UnsubscribeCommand(message.Conversation.Id, gameId, modId)))
                        {
                            await message.ReplyWith("Successful!");
                            return;
                        }
                    }
                    else
                    {
                        await message.ReplyWith("Failed!");
                        return;
                    }
                }

                const string about = "about";
                if (command.StartsWith(about))
                {
                    var uptime = _clock.GetCurrentInstant() - Process.GetCurrentProcess().StartTime.ToUniversalTime().ToInstant();
                    var subscriptionCount = await _subscriptionQueries.GetAllAsync().CountAsync();
                    var embed = AttachmentHelper.About(subscriptionCount, uptime);
                    await message.ReplyWith(new BotMessage { Attachments = { embed } });
                }

                const string subscriptions = "subscriptions";
                if (command.StartsWith(subscriptions))
                {
                    var subscriptionList = await _subscriptionQueries.GetAllAsync().Where(s => s.ChannelId == message.Conversation.Id).ToListAsync();
                    if (subscriptionList.Count > 0)
                    {
                        await message.ReplyWith($@"Subscriptions:
```
{string.Join('\n', subscriptionList.Select(s => $"Game: {s.NexusModsGameId}; Mod: {s.NexusModsModId}"))}
```");
                    }
                    else
                    {
                        await message.ReplyWith("No subscriptions found!");
                    }
                }

                const string help = "help";
                if (command.StartsWith(help))
                {
                    await message.ReplyWith(@"help
about
subscriptions
subscribe [Game Id] [Mod Id]
unsubscribe [Game Id] [Mod Id]");
                }
            }
        }

        public void Dispose()
        {
            _bot.Dispose();
        }
    }
}