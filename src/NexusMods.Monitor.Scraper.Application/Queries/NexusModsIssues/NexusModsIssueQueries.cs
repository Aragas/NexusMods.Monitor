using NexusMods.Monitor.Shared.Common;

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
            HttpResponseMessage response;

            try
            {
                response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync(
                    $"issues/id?gameId={gameIdRequest}&modId={modIdRequest}",
                    HttpCompletionOption.ResponseHeadersRead,
                    ct);
            }
            catch (Exception e) when (e is TaskCanceledException)
            {
                yield break;
            }

            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var contentStream = await response.Content.ReadAsStreamAsync(ct);
                var data = await _jsonSerializer.DeserializeAsync<IssueDTO[]?>(contentStream, ct) ?? Array.Empty<IssueDTO>();
                foreach (var tuple in data)
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

            response.Dispose();
        }

        public async Task<NexusModsIssueContentViewModel?> GetContentAsync(uint issueId, CancellationToken ct = default)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync(
                    $"issues/content?issueId={issueId}",
                    HttpCompletionOption.ResponseHeadersRead,
                    ct);
            }
            catch (Exception e) when (e is TaskCanceledException)
            {
                return null;
            }

            try
            {
                if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
                {
                    var content = await response.Content.ReadAsStreamAsync(ct);
                    if (await _jsonSerializer.DeserializeAsync<IssueContentDTO?>(content, ct) is { } tuple)
                    {
                        var (id, author, authorUrl, avatarUrl, content_, instant) = tuple;
                        return new NexusModsIssueContentViewModel(id, author, authorUrl, avatarUrl, content_, instant);
                    }
                }
                return null;
            }
            finally
            {
                response.Dispose();
            }
        }

        public async IAsyncEnumerable<NexusModsIssueReplyViewModel> GetRepliesAsync(uint issueId, [EnumeratorCancellation] CancellationToken ct = default)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync(
                    $"issues/replies?issueId={issueId}",
                    HttpCompletionOption.ResponseHeadersRead,
                    ct);
            }
            catch (Exception e) when (e is TaskCanceledException)
            {
                yield break;
            }

            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var contentStream = await response.Content.ReadAsStreamAsync(ct);
                var data = await _jsonSerializer.DeserializeAsync<IssueReplyDTO[]?>(contentStream, ct) ?? Array.Empty<IssueReplyDTO>();
                foreach (var tuple in data)
                {
                    var (id, author, authorUrl, avatarUrl, content_, instant) = tuple;
                    yield return new NexusModsIssueReplyViewModel(id, author, authorUrl, avatarUrl, content_, instant);
                }
            }

            response.Dispose();
        }

        private sealed record IssueDTO(string GameDomain,
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