using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Enbiso.NLib.EventBus;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Bot.Discord.Host.Options;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Host.BackgroundServices
{
    /// <summary>
    /// Manages the Discord connection.
    /// </summary>
    public sealed class DiscordService : IHostedService, IDisposable
    {
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commands;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;
        private readonly DiscordOptions _options;
        private readonly IEventSubscriber _eventSubscriber;

        public DiscordService(DiscordSocketClient discordSocketClient,
            CommandService commands,
            IServiceScopeFactory scopeFactory,
            ILogger<DiscordService> loggerService,
            IOptions<DiscordOptions> options,
            IEventSubscriber eventSubscriber)
        {
            _commands = commands;
            _scopeFactory = scopeFactory;
            _logger = loggerService;
            _options = options.Value;
            _eventSubscriber = eventSubscriber;

            _discordSocketClient = discordSocketClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            await _commands.AddModulesAsync(typeof(DiscordCommands).Assembly, scope.ServiceProvider);

            if (_discordSocketClient.ConnectionState != ConnectionState.Disconnecting && _discordSocketClient.ConnectionState != ConnectionState.Disconnected)
                return;

            var token = _options.BotToken;
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Error while getting Discord.BotToken! Check your configuration file.");
                return;
            }

            _discordSocketClient.MessageReceived += Bot_MessageReceived;
            _discordSocketClient.Log += Bot_Log;

            await _discordSocketClient.LoginAsync(TokenType.Bot, token);
            await _discordSocketClient.StartAsync();

            await _eventSubscriber.Subscribe();

            _logger.LogWarning("Started Discord Bot.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _discordSocketClient.MessageReceived -= Bot_MessageReceived;
            _discordSocketClient.Log -= Bot_Log;
            await _discordSocketClient.StopAsync();
            _logger.LogWarning("Stopped Discord Bot.");

            _eventSubscriber.Dispose();
        }

        private Task Bot_Log(LogMessage arg)
        {
            switch (arg.Severity)
            {
                case LogSeverity.Critical:
                    _logger.LogCritical(arg.Exception, arg.Message);
                    break;
                case LogSeverity.Error:
                    _logger.LogError(arg.Exception, arg.Message);
                    break;
                case LogSeverity.Warning:
                    _logger.LogWarning(arg.Exception, arg.Message);
                    break;
                case LogSeverity.Info:
                    _logger.LogInformation(arg.Exception, arg.Message);
                    break;
                case LogSeverity.Verbose:
                    _logger.LogTrace(arg.Exception, arg.Message);
                    break;
                case LogSeverity.Debug:
                    _logger.LogDebug(arg.Exception, arg.Message);
                    break;

                default:
                    _logger.LogWarning("Incorrect LogMessage.Severity - {arg}, {message}", (int)arg.Severity, arg.Message);
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task Bot_MessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;
            if (message.Channel is IPrivateChannel) return;

            var argPos = 0;
            if (message.HasStringPrefix("!nmm ", ref argPos) || message.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos))
            {
                using var scope = _scopeFactory.CreateScope();
                var context = new SocketCommandContext(_discordSocketClient, message);
                var result = await _commands.ExecuteAsync(context, argPos, scope.ServiceProvider);

                if (result.Error.HasValue && result.Error.Value != CommandError.UnknownCommand)
                {
                    _logger.LogError(result.ToString());
                    await context.Message.AddReactionAsync(new Emoji("⁉️"));
                }
                else if (result.Error.HasValue && result.Error.Value == CommandError.UnknownCommand)
                {
                    await context.Message.AddReactionAsync(new Emoji("❓"));
                }
            }
        }

        public void Dispose()
        {
            _discordSocketClient.Dispose();
        }
    }
}