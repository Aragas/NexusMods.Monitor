using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsGames;
using NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsMods;
using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace NexusMods.Monitor.Subscriptions.Application.Queries.Subscriptions
{
    public sealed class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly ILogger _logger;
        private readonly SubscriptionDb _context;
        private readonly INexusModsGameQueries _nexusModsGameQueries;
        private readonly INexusModsModQueries _nexusModsModQueries;

        public SubscriptionQueries(ILogger<SubscriptionQueries> logger, SubscriptionDb context, INexusModsGameQueries nexusModsGameQueries, INexusModsModQueries nexusModsModQueries)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _nexusModsModQueries = nexusModsModQueries ?? throw new ArgumentNullException(nameof(nexusModsModQueries));
        }

        public IAsyncEnumerable<SubscriptionViewModel> GetSubscriptionsAsync(CancellationToken ct = default) => _context.SubscriptionEntities
            .AsNoTracking()
            .ToAsyncEnumerable()
            .SelectAwait(async x =>
            {
                var game = await _nexusModsGameQueries.GetAsync(x.NexusModsGameId, ct);
                if (game is null)
                {
                    _logger.LogError("Subscription with Id {Id} provided invalid game id {GameId}", x.SubscriberId, x.NexusModsGameId);
                    return new SubscriptionViewModel(x.SubscriberId, x.NexusModsGameId, x.NexusModsModId, "ERROR", "ERROR");
                }

                var mod = await _nexusModsModQueries.GetAsync(x.NexusModsGameId, x.NexusModsModId, ct);
                if (mod is null)
                {
                    _logger.LogError("Subscription with Id {Id} provided invalid mod id {ModId}", x.SubscriberId, x.NexusModsModId);
                    return new SubscriptionViewModel(x.SubscriberId, x.NexusModsGameId, x.NexusModsModId, game.Name, "ERROR");
                }

                return new SubscriptionViewModel(x.SubscriberId, x.NexusModsGameId, x.NexusModsModId, game.Name, mod.Name);
            });
    }
}