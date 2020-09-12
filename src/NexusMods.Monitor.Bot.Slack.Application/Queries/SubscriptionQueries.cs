﻿using NexusMods.Monitor.Bot.Slack.Domain.AggregatesModel.SubscriptionAggregate;

using System;
using System.Collections.Generic;
using System.Linq;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries
{
    public sealed class SubscriptionQueries : ISubscriptionQueries
    {
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionQueries(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository ?? throw new ArgumentNullException(nameof(subscriptionRepository));
        }

        public IAsyncEnumerable<SubscriptionViewModel> GetAllAsync() => _subscriptionRepository.GetAllAsync()
            .Select(x => new SubscriptionViewModel(x.ChannelId, x.NexusModsGameId, x.NexusModsModId));
    }
}