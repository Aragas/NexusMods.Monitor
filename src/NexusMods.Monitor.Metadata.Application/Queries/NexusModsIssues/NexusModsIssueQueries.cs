using AngleSharp;
using AngleSharp.Dom;

using ComposableAsync;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Metadata.Application.Queries.NexusModsGames;
using NexusMods.Monitor.Metadata.Application.Queries.NexusModsMods;

using RateLimiter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.NexusModsIssues
{
    public sealed class NexusModsIssueQueries : INexusModsIssueQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly INexusModsGameQueries _nexusModsGameQueries;
        private readonly INexusModsModQueries _nexusModsModQueries;
        private readonly TimeLimiter _timeLimiterIssues;
        private readonly TimeLimiter _timeLimiterIssueReplies;

        public NexusModsIssueQueries(IHttpClientFactory httpClientFactory, IMemoryCache cache, INexusModsGameQueries nexusModsGameQueries, INexusModsModQueries nexusModsModQueries)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _nexusModsModQueries = nexusModsModQueries ?? throw new ArgumentNullException(nameof(nexusModsModQueries));

            var timeLimiterIssuesConstraint1 = new CountByIntervalAwaitableConstraint(30, TimeSpan.FromMinutes(1));
            var timeLimiterIssuesConstraint2 = new CountByIntervalAwaitableConstraint(1, TimeSpan.FromMilliseconds(500));
            _timeLimiterIssues = TimeLimiter.Compose(timeLimiterIssuesConstraint1, timeLimiterIssuesConstraint2);

            var timeLimiterIssueRepliesConstraint1 = new CountByIntervalAwaitableConstraint(10, TimeSpan.FromMinutes(1));
            var timeLimiterIssueRepliesConstraint2 = new CountByIntervalAwaitableConstraint(1, TimeSpan.FromMilliseconds(500));
            _timeLimiterIssueReplies = TimeLimiter.Compose(timeLimiterIssueRepliesConstraint1, timeLimiterIssueRepliesConstraint2);
        }

        public async IAsyncEnumerable<NexusModsIssueRootViewModel> GetAllAsync(uint gameId, uint modId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var game = await _nexusModsGameQueries.GetAsync(gameId, ct);
            var gameDomain = game?.DomainName ?? "ERROR";
            var gameName = game?.Name ??"ERROR";

            var mod = await _nexusModsModQueries.GetAsync(gameDomain, modId, ct);
            var modName = mod?.Name ?? "ERROR";

            var key = $"issues_{gameId},{modId}";
            if (!_cache.TryGetValue(key, out NexusModsIssueRootViewModel[] cacheEntry))
            {
                var issueRoots = new List<NexusModsIssueRootViewModel>();
                for (var page = 1; ; page++)
                {
                    await _timeLimiterIssues;

                    using var response = await _httpClientFactory.CreateClient().GetAsync(
                        $"https://www.nexusmods.com/Core/Libs/Common/Widgets/ModBugsTab?RH_ModBugsTab=game_id:{gameId},id:{modId},sort_by:last_reply,order:DESC,page:{page}", ct);
                    var content = await response.Content.ReadAsStringAsync(ct);

                    var config = Configuration.Default.WithDefaultLoader();
                    var context = BrowsingContext.New(config);
                    var document = await context.OpenAsync(request => request.Content(content), ct);

                    var forumBugs = document.Body?.GetElementsByClassName("forum-bugs").FirstOrDefault();
                    foreach (var issueElement in forumBugs?.GetElementsByTagName("tbody").FirstOrDefault()?.Children ?? Enumerable.Empty<IElement>())
                    {
                        var comment = new NexusModsIssueRootViewModel(gameDomain, gameId, modId, gameName, modName, NexusModsIssueViewModel.FromElement(issueElement));
                        issueRoots.Add(comment);
                        yield return comment;
                    }

                    var pageElement = forumBugs?.GetElementsByClassName("page-selected mfp-prevent-close");
                    if (pageElement is null || !int.TryParse(pageElement.FirstOrDefault()?.ToText() ?? "ERROR", out var p) || page != p)
                        break;
                }

                cacheEntry = issueRoots.Distinct(new NexusModsIssueRootViewModelComparer()).ToArray();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                _cache.Set(key, cacheEntry, cacheEntryOptions);

                yield break;
            }

            foreach (var nexusModsCommentRoot in cacheEntry)
                yield return nexusModsCommentRoot;
        }

        public async Task<NexusModsIssueContentViewModel?> GetContentAsync(uint issueId, CancellationToken ct = default)
        {
            var document = await GetModBugReplyListAsync(issueId, ct);
            var commentTags = document.Body?.GetElementsByClassName("comments").FirstOrDefault()?.GetElementsByClassName("comment") ?? Enumerable.Empty<IElement>();
            return commentTags.Select(NexusModsIssueContentViewModel.FromElement).FirstOrDefault();
        }

        public async IAsyncEnumerable<NexusModsIssueReplyViewModel> GetRepliesAsync(uint issueId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var document = await GetModBugReplyListAsync(issueId, ct);
            var commentTags = document.Body?.GetElementsByClassName("comments").FirstOrDefault()?.GetElementsByClassName("comment") ?? Enumerable.Empty<IElement>();
            foreach (var issueReply in commentTags.Skip(1))
                yield return NexusModsIssueReplyViewModel.FromElement(issueReply);
        }


        private async Task<IDocument> GetModBugReplyListAsync(uint issueId, CancellationToken ct = default)
        {
            if (!_cache.TryGetValue($"issue_replies_{issueId}", out IDocument? cacheEntry))
            {
                await _timeLimiterIssueReplies;

                using var response = await _httpClientFactory.CreateClient().PostAsync(
                    "https://www.nexusmods.com/Core/Libs/Common/Widgets/ModBugReplyList",
                    new FormUrlEncodedContent(new[] { new KeyValuePair<string?, string?>("issue_id", issueId.ToString()) }), ct);
                var content = await response.Content.ReadAsStringAsync(ct);

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);
                cacheEntry = await context.OpenAsync(request => request.Content(content), ct);

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
                _cache.Set(issueId.ToString(), cacheEntry, cacheEntryOptions);
            }

            return cacheEntry;
        }
    }
}