﻿using Discord;
using Discord.Commands;

using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application;
using NexusMods.Monitor.Bot.Discord.Application.Commands;
using NexusMods.Monitor.Bot.Discord.Application.Queries;

using NodaTime;
using NodaTime.Extensions;

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Host
{
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public sealed class DiscordCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _loggerService;
        private readonly IMediator _mediator;
        private readonly ISubscriptionQueries _subscriptionQueries;
        private readonly IClock _clock;

        public DiscordCommands(ILogger<DiscordCommands> loggerService, IMediator mediator, ISubscriptionQueries subscriptionQueries, IClock clock)
        {
            _loggerService = loggerService ?? throw new ArgumentNullException(nameof(loggerService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _subscriptionQueries = subscriptionQueries ?? throw new ArgumentNullException(nameof(subscriptionQueries));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        [Command("help")]
        public async Task Help()
        {
            _loggerService.LogInformation("Received 'help' command from user '{User}'.", Context.User.ToString());

            await Context.User.SendMessageAsync(@"help
about
subscriptions
subscribe [Game Id] [Mod Id]
unsubscribe [Game Id] [Mod Id]");
        }

        [Command("about")]
        public async Task About()
        {
            _loggerService.LogInformation("Received 'about' command from user '{User}'.", Context.User.ToString());

            var uptime = _clock.GetCurrentInstant() - Process.GetCurrentProcess().StartTime.ToUniversalTime().ToInstant();
            var subscriptionCount = await _subscriptionQueries.GetAllAsync().CountAsync();
            var embed = EmbedHelper.About(Context.Client.Guilds.Count, subscriptionCount, uptime);

            if (Context.IsPrivate)
            {
                await Context.User.SendMessageAsync(embed: embed);
                return;
            }

            await Context.Channel.SendMessageAsync(embed: embed);
        }

        [Command("subscriptions")]
        public async Task Subscriptions()
        {
            if (Context.IsPrivate)
            {
                _loggerService.LogWarning("Received 'subscriptions' in a private channel from user '{User}'.", Context.User.ToString());
                return;
            }

            _loggerService.LogInformation("Received 'subscriptions' command from user '{User}'.", Context.User.ToString());


            var subscriptions = await _subscriptionQueries.GetAllAsync().Where(s => s.ChannelId == Context.Channel.Id).ToListAsync();
            if (subscriptions.Count > 0)
            {
                await Context.Channel.SendMessageAsync($@"Subscriptions:
```
{string.Join('\n', subscriptions.Select(s => $"Game: {s.NexusModsGameId}; Mod: {s.NexusModsModId}"))}
```");
            }
            else
            {
                await Context.Channel.SendMessageAsync("No subscriptions found!");
            }
        }

        [Command("subscribe")]
        public async Task Subscribe(uint gameId, uint modId)
        {
            if (Context.IsPrivate)
            {
                _loggerService.LogWarning("Received 'subscribe' in a private channel from user '{User}'.",
                    Context.User.ToString());
                return;
            }

            _loggerService.LogInformation("Received 'subscribe' command from user '{User}'.", Context.User.ToString());


            if (await _mediator.Send(new SubscribeCommand(Context.Channel.Id, gameId, modId)))
                await Context.Message.AddReactionAsync(new Emoji("✅"));
            else
                await Context.Message.AddReactionAsync(new Emoji("❎"));
        }

        [Command("unsubscribe")]
        public async Task Unsubscribe(uint gameId, uint modId)
        {
            if (Context.IsPrivate)
            {
                _loggerService.LogWarning("Received 'unsubscribe' in a private channel from user '{User}'.", Context.User.ToString());
                return;
            }

            _loggerService.LogInformation("Received 'unsubscribe' command from user '{User}'.", Context.User.ToString());


            if (await _mediator.Send(new UnsubscribeCommand(Context.Channel.Id, gameId, modId)))
                await Context.Message.AddReactionAsync(new Emoji("✅"));
            else
                await Context.Message.AddReactionAsync(new Emoji("❎"));
        }
    }
}