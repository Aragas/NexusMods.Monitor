using Enbiso.NLib.EventBus;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Bot.Slack.Application.Options;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NexusMods.Monitor.Bot.Slack.Domain.AggregatesModel.SubscriptionAggregate;
using NodaTime;
using NodaTime.Extensions;
using SlackNet.Bot;

namespace NexusMods.Monitor.Bot.Slack.Application.BackgroundServices
{
    /// <summary>
    /// Manages the Discord connection.
    /// </summary>
    public sealed class SlackService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly SlackBot _bot;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IClock _clock;
        private readonly SlackOptions _options;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IEventSubscriber _eventSubscriber;

        public SlackService(ILogger<SlackService> logger,
            SlackBot bot,
            IServiceScopeFactory scopeFactory,
            IClock clock,
            IOptions<SlackOptions> options,
            ISubscriptionRepository subscriptionRepository,
            IEventSubscriber eventSubscriber)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(bot));
            _bot = bot ?? throw new ArgumentNullException(nameof(bot));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
            _options = options.Value ?? throw new ArgumentNullException(nameof(bot));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
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

        private async void Bot_OnMessageReceived(object sender, IMessage message)
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
                    if (args.Length == 2)
                    {
                        var gameId = uint.Parse(args[0]);
                        var modId = uint.Parse(args[1]);

                        await _subscriptionRepository.SubscribeAsync(message.Conversation.Id, gameId, modId);
                        if (await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync())
                            await message.ReplyWith("Successful!");
                        else
                            await message.ReplyWith("Failed!");
                    }
                }

                const string unsubscribe = "unsubscribe ";
                if (command.StartsWith(unsubscribe))
                {
                    var argsText = command.Remove(0, unsubscribe.Length);
                    var args = argsText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (args.Length == 2)
                    {
                        var gameId = uint.Parse(args[0]);
                        var modId = uint.Parse(args[1]);

                        await _subscriptionRepository.UnsubscribeAsync(message.Conversation.Id, gameId, modId);
                        if (await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync())
                            await message.ReplyWith("Successful!");
                        else
                            await message.ReplyWith("Failed!");
                    }
                }

                const string about = "about";
                if (command.StartsWith(about))
                {
                    var uptime = _clock.GetCurrentInstant() - Process.GetCurrentProcess().StartTime.ToUniversalTime().ToInstant();
                    var subscriptionCount = await _subscriptionRepository.GetAllAsync().CountAsync();
                    var embed = AttachmentHelper.About(subscriptionCount, uptime);
                    await message.ReplyWith(new BotMessage { Attachments = { embed } });
                }
            }

            /*
            // Ignore system messages and messages from bots
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            if (message.Channel is IPrivateChannel)
            {
                return;
            }

            var argPos = 0;
            if (message.HasStringPrefix("!nmm ", ref argPos) || message.HasMentionPrefix(_bot.CurrentUser, ref argPos))
            {
                using var scope = _scopeFactory.CreateScope();
                var context = new SocketCommandContext(_bot, message);
                var result = await _commands.ExecuteAsync(context, argPos, scope.ServiceProvider);

                if (result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand)
                {
                    _loggerService.LogError(result.ToString());
                    await context.Message.AddReactionAsync(new Emoji("⁉️"));
                }
                else if (result.Error.HasValue && result.Error.Value == CommandError.UnknownCommand)
                {
                    await context.Message.AddReactionAsync(new Emoji("❓"));
                }
            }
            */
        }

        public void Dispose()
        {
            _bot?.Dispose();
        }
    }
}