using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.Caching.Memory;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.NexusModsGameAggregate;
using NexusMods.Monitor.Scraper.Infrastructure.Comparators;
using NexusMods.Monitor.Scraper.Infrastructure.ComposableAsync;
using NexusMods.Monitor.Scraper.Infrastructure.Extensions;
using NexusMods.Monitor.Scraper.Infrastructure.Models.Issues;
using NexusMods.Monitor.Scraper.Infrastructure.RateLimiter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Repositories
{
    public class NexusModsIssuesRepository : INexusModsIssuesRepository
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        private readonly INexusModsGameRepository _nexusModsGameRepository;
        private readonly TimeLimiter _timeLimiterIssues;
        private readonly TimeLimiter _timeLimiterIssueReplies;

        public NexusModsIssuesRepository(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache, INexusModsGameRepository nexusModsGameRepository)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _nexusModsGameRepository = nexusModsGameRepository ?? throw new ArgumentNullException(nameof(nexusModsGameRepository));

            var timeLimiterIssuesConstraint1 = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromMinutes(1)); // Create first constraint: max 30 times by minute
            var timeLimiterIssuesConstraint2 = new CountByIntervalAwaitableConstraint(1, TimeSpan.FromMilliseconds(500)); // Create second constraint: one time each 500 ms
            _timeLimiterIssues = TimeLimiter.Compose(timeLimiterIssuesConstraint1, timeLimiterIssuesConstraint2);

            var timeLimiterIssueRepliesConstraint1 = new CountByIntervalAwaitableConstraint(10, TimeSpan.FromMinutes(1)); // Create first constraint: max 30 times by minute
            var timeLimiterIssueRepliesConstraint2 = new CountByIntervalAwaitableConstraint(1, TimeSpan.FromMilliseconds(500)); // Create second constraint: one time each 500 ms
            _timeLimiterIssueReplies = TimeLimiter.Compose(timeLimiterIssueRepliesConstraint1, timeLimiterIssueRepliesConstraint2);
        }

        public async IAsyncEnumerable<NexusModsIssueRoot> GetIssuesAsync(uint gameId, uint modId)
        {
            var game = await _nexusModsGameRepository.GetAsync(gameId);
            var gameIdText = game?.DomainName ?? "ERROR";

            var key = $"issues_{gameId},{modId}";
            if (!_memoryCache.TryGetValue(key, out NexusModsIssueRoot[] cacheEntry))
            {
                var issueRoots = new List<NexusModsIssueRoot>();
                for (var page = 1;; page++)
                {
                    await _timeLimiterIssues;

                    using var response = await _httpClientFactory.CreateClient().GetAsync(
                        $"https://www.nexusmods.com/Core/Libs/Common/Widgets/ModBugsTab?RH_ModBugsTab=game_id:{gameId},id:{modId},sort_by:last_reply,order:DESC,page:{page}");
                    var content = await response.Content.ReadAsStringAsync();

                    var config = Configuration.Default.WithDefaultLoader();
                    var context = BrowsingContext.New(config);
                    var document = await context.OpenAsync(request => request.Content(content));

                    var forumBugs = document.Body.GetElementsByClassName("forum-bugs").FirstOrDefault();
                    foreach (var issueElement in forumBugs?.GetElementsByTagName("tbody").FirstOrDefault()?.Children ?? Enumerable.Empty<IElement>())
                    {
                        var comment = new NexusModsIssueRoot(gameIdText, gameId, modId, new NexusModsIssue(issueElement));
                        issueRoots.Add(comment);
                        yield return comment;
                    }

                    var pageElement = forumBugs?.GetElementsByClassName("page-selected mfp-prevent-close");
                    if (pageElement is null || !int.TryParse(pageElement.FirstOrDefault()?.ToText() ?? "ERROR", out var p) || page != p)
                        break;
                }

                cacheEntry = issueRoots.Distinct(new NexusModsIssueRootComparer()).ToArray();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                _memoryCache.Set(key, cacheEntry, cacheEntryOptions);

                yield break;
            }

            foreach (var nexusModsCommentRoot in cacheEntry)
                yield return nexusModsCommentRoot;
        }

        public async Task<NexusModsIssueContent?> GetIssueContentAsync(uint issueId)
        {
            var document = await GetModBugReplyListAsync(issueId);
            var commentTags = document.Body.GetElementsByClassName("comments").FirstOrDefault()?.GetElementsByClassName("comment") ?? Enumerable.Empty<IElement>();
            return commentTags.Select(x => new NexusModsIssueContent(x)).FirstOrDefault();
        }

        public async IAsyncEnumerable<NexusModsIssueReply> GetIssueRepliesAsync(uint issueId)
        {
            var document = await GetModBugReplyListAsync(issueId);
            var commentTags = document.Body.GetElementsByClassName("comments").FirstOrDefault()?.GetElementsByClassName("comment") ?? Enumerable.Empty<IElement>();
            foreach (var issueReply in commentTags.Skip(1))
                yield return new NexusModsIssueReply(issueReply);
        }


        private async Task<IDocument> GetModBugReplyListAsync(uint issueId)
        {
            if (!_memoryCache.TryGetValue($"issue_replies_{issueId}", out IDocument cacheEntry))
            {
                await _timeLimiterIssueReplies;

                using var response = await _httpClientFactory.CreateClient().PostAsync(
                    "https://www.nexusmods.com/Core/Libs/Common/Widgets/ModBugReplyList",
                    new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("issue_id", issueId.ToString()) }));
                var content = await response.Content.ReadAsStringAsync();

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                cacheEntry = await context.OpenAsync(request => request.Content(content));

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSize(1)
                    .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));

                _memoryCache.Set(issueId, cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}