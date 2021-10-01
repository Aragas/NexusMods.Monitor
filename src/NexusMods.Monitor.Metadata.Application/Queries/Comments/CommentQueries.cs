using AngleSharp;
using AngleSharp.Dom;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Metadata.Application.Extensions;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;
using NexusMods.Monitor.Metadata.Application.Queries.Threads;

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
        private readonly ILogger _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _cache;
        private readonly IGameQueries _nexusModsGameQueries;
        private readonly IModQueries _nexusModsModQueries;
        private readonly IThreadQueries _nexusModsThreadQueries;

        public CommentQueries(ILogger<CommentQueries> logger, IHttpClientFactory httpClientFactory, IMemoryCache cache, IGameQueries nexusModsGameQueries, IModQueries nexusModsModQueries, IThreadQueries nexusModsThreadQueries)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _nexusModsGameQueries = nexusModsGameQueries ?? throw new ArgumentNullException(nameof(nexusModsGameQueries));
            _nexusModsModQueries = nexusModsModQueries ?? throw new ArgumentNullException(nameof(nexusModsModQueries));
            _nexusModsThreadQueries = nexusModsThreadQueries ?? throw new ArgumentNullException(nameof(nexusModsThreadQueries));
        }

        public async IAsyncEnumerable<CommentViewModel> GetAllAsync(uint gameId, uint modId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            var game = await _nexusModsGameQueries.GetAsync(gameId, ct);
            if (game is null)
                yield break;
            var gameDomain = game.DomainName;
            var gameName = game.Name;

            var mod = await _nexusModsModQueries.GetAsync(gameDomain, modId, ct);
            if (mod is null)
                yield break;
            var modName = mod.Name;

            var threadViewModel = await _nexusModsThreadQueries.GetAsync(gameId, modId, ct);
            if (threadViewModel is null)
                yield break;
            var threadId = threadViewModel.ThreadId;

            var key = $"comments_{gameId},{modId},{threadId}";
            if (!_cache.TryGetValue(key, out CommentViewModel[]? cacheEntry))
            {
                var commentRoots = new Dictionary<uint, CommentViewModel>();
                for (var page = 1; ; page++)
                {
                    using var response = await _httpClientFactory.CreateClient("NexusMods").GetAsync(
                        $"Core/Libs/Common/Widgets/CommentContainer?RH_CommentContainer=game_id:{gameId},object_id:{modId},object_type:1,thread_id:{threadId},page:{page}", ct);
                    var content = await response.Content.ReadAsStringAsync(ct);

                    var config = Configuration.Default.WithDefaultLoader();
                    var context = BrowsingContext.New(config);
                    var document = await context.OpenAsync(request => request.Content(content), ct);

                    var commentContainer = document.GetElementById("comment-container");
                    foreach (var commentElement in commentContainer?.GetElementsByTagName("ol").FirstOrDefault()?.Children ?? Enumerable.Empty<IElement>())
                    {
                        var comment = CommentViewModel.FromElement(gameDomain, gameId, modId, gameName, modName, commentElement);
                        if (commentRoots.TryGetValue(comment.Id, out var existingComment))
                        {
                            _logger.LogInformation("Comment was already added! Existing: {@ExistingComment}, new: {@NewComment}", existingComment, comment);
                        }
                        else
                        {
                            commentRoots.Add(comment.Id, comment);
                        }
                    }

                    var pageElement = commentContainer?.GetElementsByClassName("page-selected mfp-prevent-close");
                    if (pageElement is null || !int.TryParse(pageElement.FirstOrDefault()?.ToText() ?? "ERROR", out var p) || page != p)
                        break;
                }

                cacheEntry = commentRoots.Values.ToArray();
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1).SetAbsoluteExpiration(TimeSpan.FromSeconds(60));
                _cache.Set(key, cacheEntry, cacheEntryOptions);
            }

            foreach (var nexusModsCommentRoot in cacheEntry ?? Array.Empty<CommentViewModel>())
                yield return nexusModsCommentRoot;
        }
    }
}