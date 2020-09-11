using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.Caching.Memory;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsGameAggregate;
using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsThreadAggregate;
using NexusMods.Monitor.Scraper.Infrastructure.Comparators;
using NexusMods.Monitor.Scraper.Infrastructure.ComposableAsync;
using NexusMods.Monitor.Scraper.Infrastructure.Extensions;
using NexusMods.Monitor.Scraper.Infrastructure.Models.Comments;
using NexusMods.Monitor.Scraper.Infrastructure.RateLimiter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace NexusMods.Monitor.Scraper.Infrastructure.Repositories
{
    public class NexusModsCommentsRepository : INexusModsCommentsRepository
    {
        private readonly INexusModsThreadRepository _nexusModsThreadRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly INexusModsGameRepository _nexusModsGameRepository;
        private readonly TimeLimiter _timeLimiterComments;

        public NexusModsCommentsRepository(INexusModsThreadRepository nexusModsThreadRepository, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, INexusModsGameRepository nexusModsGameRepository)
        {
            _nexusModsThreadRepository = nexusModsThreadRepository ?? throw new ArgumentNullException(nameof(nexusModsThreadRepository));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _nexusModsGameRepository = nexusModsGameRepository ?? throw new ArgumentNullException(nameof(nexusModsGameRepository));

            var timeLimiterCommentsConstraint1 = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromMinutes(1));
            var timeLimiterCommentsConstraint2 = new CountByIntervalAwaitableConstraint(1, TimeSpan.FromMilliseconds(500));
            _timeLimiterComments = TimeLimiter.Compose(timeLimiterCommentsConstraint1, timeLimiterCommentsConstraint2);
        }

        public async IAsyncEnumerable<NexusModsCommentRoot> GetCommentsAsync(uint gameId, uint modId)
        {
            var game = await _nexusModsGameRepository.GetAsync(gameId);
            var gameIdText = game?.DomainName ?? "ERROR";
            var nexusModsThreadEntity = await _nexusModsThreadRepository.GetAsync(gameId, modId);

            var key = $"comments_{gameId},{modId},{nexusModsThreadEntity.ThreadId}";
            if (!_memoryCache.TryGetValue(key, out NexusModsCommentRoot[] cacheEntry))
            {
                var commentRoots = new List<NexusModsCommentRoot>();
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
                        var comment = new NexusModsCommentRoot(gameIdText, gameId, modId, new NexusModsComment(commentElement));
                        commentRoots.Add(comment);
                        yield return comment;
                    }

                    var pageElement = commentContainer?.GetElementsByClassName("page-selected mfp-prevent-close");
                    if (pageElement is null || !int.TryParse(pageElement.FirstOrDefault()?.ToText() ?? "ERROR", out var p) || page != p)
                        break;
                }

                cacheEntry = commentRoots.Distinct(new NexusModsCommentRootComparer()).ToArray();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                _memoryCache.Set(key, cacheEntry, cacheEntryOptions);

                yield break;
            }

            foreach (var nexusModsCommentRoot in cacheEntry)
                yield return nexusModsCommentRoot;
        }
    }
}