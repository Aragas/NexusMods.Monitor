﻿using NexusMods.Monitor.Shared.Common;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsGames
{
    public sealed class NexusModsGameQueries : INexusModsGameQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public NexusModsGameQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<NexusModsGameViewModel?> GetAsync(uint gameId, CancellationToken ct = default)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync(
                    $"game/id?gameId={gameId}",
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
                    if (await _jsonSerializer.DeserializeAsync<GameDTO?>(content, ct) is { } tuple)
                    {
                        var (id, name, forumUrl, url, domainName) = tuple;
                        return new NexusModsGameViewModel(id, name, forumUrl, url, domainName);
                    }
                }
                return null;
            }
            finally
            {
                response.Dispose();
            }
        }

        public async Task<NexusModsGameViewModel?> GetAsync(string gameDomain, CancellationToken ct = default)
        {
            HttpResponseMessage response;

            try
            {
                response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync(
                    $"game/domain?gameDomain={gameDomain}",
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
                    if (await _jsonSerializer.DeserializeAsync<GameDTO?>(content, ct) is { } tuple)
                    {
                        var (id, name, forumUrl, url, domainName) = tuple;
                        return new NexusModsGameViewModel(id, name, forumUrl, url, domainName);
                    }
                }
                return null;
            }
            finally
            {
                response.Dispose();
            }
        }

        private sealed record GameDTO(uint Id, string Name, string ForumUrl, string Url, string DomainName);
    }
}