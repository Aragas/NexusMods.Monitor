using NexusMods.Monitor.Shared.Application;

using NodaTime;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsComments
{
    public sealed class NexusModsCommentQueries : INexusModsCommentQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public NexusModsCommentQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async IAsyncEnumerable<NexusModsCommentRootViewModel> GetAllAsync(uint gameIdRequest, uint modIdRequest, [EnumeratorCancellation] CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync($"comments/id?gameId={gameIdRequest}&modId={modIdRequest}", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                foreach (var (gameDomain, gameId, modId, gameName, modName, id, author, authorUrl, avatarUrl, s, isSticky, isLocked, instant, nexusModsCommentReplyViewModels) in _jsonSerializer.Deserialize<CommentsDTO[]?>(content) ?? Array.Empty<CommentsDTO>())
                {
                    yield return new NexusModsCommentRootViewModel(gameDomain, gameId, modId, gameName, modName, new NexusModsCommentViewModel(id, author, authorUrl, avatarUrl, s, isSticky, isLocked, instant, nexusModsCommentReplyViewModels));
                }
            }
        }

        private record CommentsDTO(
            string GameDomain,
            uint GameId,
            uint ModId,
            string GameName,
            string ModName,
            uint Id,
            string Author,
            string AuthorUrl,
            string AvatarUrl,
            string Content,
            bool IsSticky,
            bool IsLocked,
            Instant Post,
            IReadOnlyList<NexusModsCommentReplyViewModel> Replies);
    }
}