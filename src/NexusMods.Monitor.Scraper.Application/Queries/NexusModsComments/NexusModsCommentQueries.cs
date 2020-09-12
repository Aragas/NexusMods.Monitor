using AngleSharp;
using AngleSharp.Dom;

using ComposableAsync;

using Microsoft.Extensions.Caching.Memory;

using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsGames;
using NexusMods.Monitor.Scraper.Application.Queries.NexusModsThreads;

using RateLimiter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public sealed class NexusModsCommentQueries : INexusModsCommentQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly INexusModsGameQueries _nexusModsGameQueries;
        private readonly INexusModsThreadQueries _nexusModsThreadQueries;
        private readonly TimeLimiter _timeLimiterComments;

        public NexusModsCommentQueries(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, INexusModsGameQueries nexusModsGameQueries, INexusModsThreadQueries nexusModsThreadQueries)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _nexusModsThreadQueries = nexusModsThreadQueries ?? throw new ArgumentNullException(nameof(nexusModsThreadQueries));

            var timeLimiterCommentsConstraint1 = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromMinutes(1));
            var timeLimiterCommentsConstraint2 = new CountByIntervalAwaitableConstraint(1, TimeSpan.FromMilliseconds(500));
            _timeLimiterComments = TimeLimiter.Compose(timeLimiterCommentsConstraint1, timeLimiterCommentsConstraint2);
        }

        public async IAsyncEnumerable<NexusModsCommentRootViewModel> GetAllAsync(uint gameId, uint modId)
        {
            var game = await _nexusModsGameQueries.GetAsync(gameId);
            var gameIdText = game?.DomainName ?? "ERROR";
            var nexusModsThreadEntity = await _nexusModsThreadQueries.GetAsync(gameId, modId);

            var key = $"comments_{gameId},{modId},{nexusModsThreadEntity.ThreadId}";
            if (!_memoryCache.TryGetValue(key, out NexusModsCommentRootViewModel[] cacheEntry))
            {
                var commentRoots = new List<NexusModsCommentRootViewModel>();
                for (var page = 1;; page++)
                {
                    await _timeLimiterComments;

                    using var response = await _httpClientFactory.CreateClient().GetAsync(
                        $"https://www.nexusmods.com/Core/Libs/Common/Widgets/CommentContainer?RH_CommentContainer=game_id:{gameId},object_id:{modId},object_type:1,thread_id:{nexusModsThreadEntity.ThreadId},page:{page}");
                    var content = await response.Content.ReadAsStringAsync();

                    var config = Configuration.Default.WithDefaultLoader();
                    var context = BrowsingContext.New(config);
                    var document = await context.OpenAsync(request => request.Content(content));

                    var commentContainer = document.GetElementById("comment-container");
                    foreach (var commentElement in commentContainer?.GetElementsByTagName("ol")?.FirstOrDefault()?.Children ?? Enumerable.Empty<IElement>())
                    {
                        var comment = new NexusModsCommentRootViewModel(gameIdText, gameId, modId, new NexusModsCommentViewModel(commentElement));
                        commentRoots.Add(comment);
                        yield return comment;
                    }

                    var pageElement = commentContainer?.GetElementsByClassName("page-selected mfp-prevent-close");
                    if (pageElement is null || !int.TryParse(pageElement.FirstOrDefault()?.ToText() ?? "ERROR", out var p) || page != p)
                        break;
                }

                cacheEntry = commentRoots.Distinct(new NexusModsCommentRootViewModelComparer()).ToArray();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                _memoryCache.Set(key, cacheEntry, cacheEntryOptions);

                yield break;
            }

            foreach (var nexusModsCommentRoot in cacheEntry)
                yield return nexusModsCommentRoot;
        }
    }
}