using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.Application.Queries.Issues
{
    public sealed class IssueQueries : IIssueQueries
    {
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IGameQueries _nexusModsGameQueries;
        private readonly IModQueries _nexusModsModQueries;

        public IssueQueries(ILogger<IssueQueries> logger, IHttpClientFactory httpClientFactory, IMemoryCache cache, IGameQueries nexusModsGameQueries, IModQueries nexusModsModQueries)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _nexusModsModQueries = nexusModsModQueries ?? throw new ArgumentNullException(nameof(nexusModsModQueries));
        }

        public async IAsyncEnumerable<IssueViewModel> GetAllAsync(uint gameId, uint modId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var game = await _nexusModsGameQueries.GetAsync(gameId, ct);
            var gameDomain = game?.DomainName ?? "ERROR";
            var gameName = game?.Name ?? "ERROR";

            var mod = await _nexusModsModQueries.GetAsync(gameDomain, modId, ct);
            var modName = mod?.Name ?? "ERROR";

            var key = $"issues_{gameId},{modId}";
            if (!_cache.TryGetValue(key, out IssueViewModel[]? cacheEntry))
            {
                var issueRoots = new Dictionary<uint, IssueViewModel>();
                for (var page = 1; ; page++)
                {
                    using var response = await _httpClientFactory.CreateClient("NexusMods").GetAsync(
                        $"Core/Libs/Common/Widgets/ModBugsTab?RH_ModBugsTab=game_id:{gameId},id:{modId},sort_by:last_reply,order:DESC,page:{page}", ct);
                    var content = await response.Content.ReadAsStringAsync(ct);

                    var config = Configuration.Default.WithDefaultLoader();
                    var context = BrowsingContext.New(config);
                    var document = await context.OpenAsync(request => request.Content(content), ct);

                    var forumBugs = document.Body?.GetElementsByClassName("forum-bugs").FirstOrDefault();
                    foreach (var issueElement in forumBugs?.GetElementsByTagName("tbody").FirstOrDefault()?.Children ?? Enumerable.Empty<IElement>())
                    {
                        var issue = IssueViewModel.FromElement(gameDomain, gameId, modId, gameName, modName, issueElement);
                        if (issueRoots.TryGetValue(issue.Id, out var existingIssue))
                        {
                            _logger.LogInformation("Issue was already added! Existing: {@ExistingIssue}, new: {@NewIssue}", existingIssue, issue);
                        }
                        else
                        {
                            issueRoots.Add(issue.Id, issue);
                        }
                    }

                    var pageElement = forumBugs?.GetElementsByClassName("page-selected mfp-prevent-close");
                    if (pageElement is null || !int.TryParse(pageElement.FirstOrDefault()?.ToText() ?? "ERROR", out var p) || page != p)
                        break;
                }

                cacheEntry = issueRoots.Values.ToArray();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            foreach (var nexusModsCommentRoot in cacheEntry ?? Array.Empty<IssueViewModel>())
                yield return nexusModsCommentRoot;
        }

        public async Task<IssueContentViewModel?> GetContentAsync(uint issueId, CancellationToken ct = default)
        {
            var document = await GetModBugReplyListAsync(issueId, ct);
            var commentTags = document?.Body?.GetElementsByClassName("comments").FirstOrDefault()?.GetElementsByClassName("comment") ?? Enumerable.Empty<IElement>();
            return commentTags.Select(IssueContentViewModel.FromElement).FirstOrDefault();
        }

        public async IAsyncEnumerable<IssueReplyViewModel> GetRepliesAsync(uint issueId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var document = await GetModBugReplyListAsync(issueId, ct);
            var commentTags = document?.Body?.GetElementsByClassName("comments").FirstOrDefault()?.GetElementsByClassName("comment") ?? Enumerable.Empty<IElement>();
            foreach (var issueReply in commentTags.Skip(1))
                yield return IssueReplyViewModel.FromElement(issueReply);
        }


        private async Task<IDocument?> GetModBugReplyListAsync(uint issueId, CancellationToken ct = default)
        {
            if (!_cache.TryGetValue($"issue_replies_{issueId}", out IDocument? cacheEntry))
            {
                using var response = await _httpClientFactory.CreateClient("NexusMods").PostAsync(
                    "Core/Libs/Common/Widgets/ModBugReplyList",
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