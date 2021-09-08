using NexusMods.Monitor.Shared.Application;

using NodaTime;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Application.Queries.NexusModsIssues
{
    public sealed class NexusModsIssueQueries : INexusModsIssueQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public NexusModsIssueQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async IAsyncEnumerable<NexusModsIssueRootViewModel> GetAllAsync(uint gameIdRequest, uint modIdRequest, [EnumeratorCancellation] CancellationToken ct)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync($"issues/id?gameId={gameIdRequest}&modId={modIdRequest}", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                foreach (var tuple in _jsonSerializer.Deserialize<IssueDTO[]?>(content) ?? Array.Empty<IssueDTO>())
                {
                    var (gameDomain, gameId, modId, gameName, modName, id, title, isPrivate, isClosed, (statusId, statusName), replyCount, modVersion, (priorityId, priorityName), lastPost) = tuple;
                    yield return new NexusModsIssueRootViewModel(
                        gameDomain, gameId, modId, gameName, modName,
                        new NexusModsIssueViewModel(id, title, isPrivate, isClosed,
                            new NexusModsIssueStatus(statusId, statusName),
                            replyCount, modVersion,
                            new NexusModsIssuePriority(priorityId, priorityName),
                            lastPost));
                }
            }
        }

        public async Task<NexusModsIssueContentViewModel?> GetContentAsync(uint issueId, CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync($"issues/content?issueId={issueId}", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                if (_jsonSerializer.Deserialize<IssueContentDTO?>(content) is { } tuple)
                {
                    var (id, author, authorUrl, avatarUrl, content_, instant) = tuple;
                    return new NexusModsIssueContentViewModel(id, author, authorUrl, avatarUrl, content_, instant);
                }
            }
            return null;
        }

        public async IAsyncEnumerable<NexusModsIssueReplyViewModel> GetRepliesAsync(uint issueId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync($"issues/replies?issueId={issueId}", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                foreach (var tuple in _jsonSerializer.Deserialize<IssueReplyDTO[]?>(content) ?? Array.Empty<IssueReplyDTO>())
                {
                    var (id, author, authorUrl, avatarUrl, content_, instant) = tuple;
                    yield return new NexusModsIssueReplyViewModel(id, author, authorUrl, avatarUrl, content_, instant);
                }
            }
        }

        public sealed record IssueDTO(string GameDomain,
            uint GameId,
            uint ModId,
            string GameName,
            string ModName,
            uint Id,
            string Title,
            bool IsPrivate,
            bool IsClosed,
            NexusModsIssueStatus Status,
            uint ReplyCount,
            string ModVersion,
            NexusModsIssuePriority Priority,
            Instant LastPost);
        private sealed record IssueContentDTO(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant Time);
        private sealed record IssueReplyDTO(uint Id, string Author, string AuthorUrl, string AvatarUrl, string Content, Instant Time);
    }
}