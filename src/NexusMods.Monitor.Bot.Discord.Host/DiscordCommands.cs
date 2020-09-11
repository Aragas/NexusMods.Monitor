using Discord;
using Discord.Commands;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Bot.Discord.Application;
using NexusMods.Monitor.Bot.Discord.Domain.AggregatesModel.SubscriptionAggregate;

using NodaTime;
using NodaTime.Extensions;

using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Discord.Host
{
    [RequireUserPermission(GuildPermission.Administrator, Group = "Permission")]
    [RequireOwner(Group = "Permission")]
    public class DiscordCommands : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger _loggerService;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IClock _clock;

        public DiscordCommands(ILogger<DiscordCommands> loggerService, ISubscriptionRepository subscriptionRepository, IClock clock)
        {
            _loggerService = loggerService;
            _subscriptionRepository = subscriptionRepository;
            _clock = clock;
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
            var subscriptionCount = await _subscriptionRepository.GetAllAsync().CountAsync();
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


            var subscriptions = await _subscriptionRepository.GetAllAsync().Where(s => s.ChannelId == Context.Channel.Id).ToListAsync();
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


            await _subscriptionRepository.SubscribeAsync(Context.Channel.Id, gameId, modId);
            if (await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync())
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


            await _subscriptionRepository.UnsubscribeAsync(Context.Channel.Id, gameId, modId);
            if (await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync())
                await Context.Message.AddReactionAsync(new Emoji("✅"));
            else
                await Context.Message.AddReactionAsync(new Emoji("❎"));
        }
    }
}