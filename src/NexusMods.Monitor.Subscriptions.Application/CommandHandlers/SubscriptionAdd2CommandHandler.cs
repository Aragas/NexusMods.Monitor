using MediatR;

using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsGames;
using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Application.CommandHandlers
{
    public sealed class SubscriptionAdd2CommandHandler : IRequestHandler<SubscriptionAdd2Command, bool>
    {
        private readonly ILogger _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INexusModsGameQueries _nexusModsGameQueries;

        public SubscriptionAdd2CommandHandler(ILogger<SubscriptionAdd2CommandHandler> logger, ISubscriptionRepository subscriptionRepository, INexusModsGameQueries nexusModsGameQueries)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
        }

        public async Task<bool> Handle(SubscriptionAdd2Command message, CancellationToken ct)
        {
            var uri = new Uri(message.NexusModsUrl, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(new Uri("https://www.nexusmods.com"), uri);

            var absolutePath = Uri.UnescapeDataString(uri.AbsolutePath);
            var segments = absolutePath.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (segments.Length != 3 || !uint.TryParse(segments.Last(), out var nexusModsModId))
            {
                _logger.LogError("Subscription with Id {Id} provided invalid NexusModsUrl {nexusModsUrl}.", message.SubscriberId, message.NexusModsUrl);
                return false;
            }

            var gameDomain = segments.First();
            var game = await _nexusModsGameQueries.GetAsync(gameDomain, ct);
            if (game is null)
            {
                _logger.LogError("Subscription with Id {Id} provided invalid game domain {gameDomain}.", message.SubscriberId, gameDomain);
                return false;
            }

            var nexusModsGameId = game.Id;

            var existingSubscription = await _subscriptionRepository.GetAsync(message.SubscriberId, nexusModsGameId, nexusModsModId);
            if (existingSubscription is { })
            {
                _logger.LogError("Subscription with Id {Id} already exists.", message.SubscriberId);
                return false;
            }

            var subscriptionEntity = new SubscriptionEntity(message.SubscriberId, nexusModsGameId, nexusModsModId);
            _subscriptionRepository.Add(subscriptionEntity);

            return await _subscriptionRepository.UnitOfWork.SaveEntitiesAsync(ct);
        }
    }
}