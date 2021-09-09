﻿using AngleSharp;
using AngleSharp.Dom;

using ComposableAsync;

using Microsoft.Extensions.Caching.Memory;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;
using NexusMods.Monitor.Metadata.Application.Queries.Threads;

using RateLimiter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NexusMods.Monitor.Metadata.Application.Queries.Comments
{
    public sealed class CommentQueries : ICommentQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IGameQueries _nexusModsGameQueries;
        private readonly IModQueries _nexusModsModQueries;
        private readonly IThreadQueries _nexusModsThreadQueries;
        private readonly TimeLimiter _timeLimiterComments;

        public CommentQueries(IHttpClientFactory httpClientFactory, IMemoryCache cache, IGameQueries nexusModsGameQueries, IModQueries nexusModsModQueries, IThreadQueries nexusModsThreadQueries)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _nexusModsModQueries = nexusModsModQueries ?? throw new ArgumentNullException(nameof(nexusModsModQueries));
            _nexusModsThreadQueries = nexusModsThreadQueries ?? throw new ArgumentNullException(nameof(nexusModsThreadQueries));

            var timeLimiterCommentsConstraint1 = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromMinutes(1));
            var timeLimiterCommentsConstraint2 = new CountByIntervalAwaitableConstraint(1, TimeSpan.FromMilliseconds(500));
            _timeLimiterComments = TimeLimiter.Compose(timeLimiterCommentsConstraint1, timeLimiterCommentsConstraint2);
        }

        public async IAsyncEnumerable<CommentViewModel> GetAllAsync(uint gameId, uint modId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var game = await _nexusModsGameQueries.GetAsync(gameId, ct);
            var gameDomain = game?.DomainName ?? "ERROR";
            var gameName = game?.Name ?? "ERROR";

            var mod = await _nexusModsModQueries.GetAsync(gameDomain, modId, ct);
            var modName = mod?.Name ?? "ERROR";

            var (_, _, threadId) = await _nexusModsThreadQueries.GetAsync(gameId, modId, ct);

            var key = $"comments_{gameId},{modId},{threadId}";
            if (!_cache.TryGetValue(key, out CommentViewModel[] cacheEntry))
            {
                var commentRoots = new List<CommentViewModel>();
                for (var page = 1; ; page++)
                {
                    await _timeLimiterComments;

                    using var response = await _httpClientFactory.CreateClient().GetAsync(
                        $"https://www.nexusmods.com/Core/Libs/Common/Widgets/CommentContainer?RH_CommentContainer=game_id:{gameId},object_id:{modId},object_type:1,thread_id:{threadId},page:{page}", ct);
                    var content = await response.Content.ReadAsStringAsync(ct);

                    var config = Configuration.Default.WithDefaultLoader();
                    var context = BrowsingContext.New(config);
                    var document = await context.OpenAsync(request => request.Content(content), ct);

                    var commentContainer = document.GetElementById("comment-container");
                    foreach (var commentElement in commentContainer?.GetElementsByTagName("ol")?.FirstOrDefault()?.Children ?? Enumerable.Empty<IElement>())
                    {
                        var comment = CommentViewModel.FromElement(gameDomain, gameId, modId, gameName, modName, commentElement);
                        commentRoots.Add(comment);
                        yield return comment;
                    }

                    var pageElement = commentContainer?.GetElementsByClassName("page-selected mfp-prevent-close");
                    if (pageElement is null || !int.TryParse(pageElement.FirstOrDefault()?.ToText() ?? "ERROR", out var p) || page != p)
                        break;
                }

                cacheEntry = commentRoots.Distinct(new CommentViewModelComparer()).ToArray();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                _cache.Set(key, cacheEntry, cacheEntryOptions);

                yield break;
            }

            foreach (var nexusModsCommentRoot in cacheEntry)
                yield return nexusModsCommentRoot;
        }
    }
}