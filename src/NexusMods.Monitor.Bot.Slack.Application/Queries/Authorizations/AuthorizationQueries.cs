﻿using NexusMods.Monitor.Shared.Application;

using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries.Authorizations
{
    public sealed class AuthorizationQueries : IAuthorizationQueries
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly DefaultJsonSerializer _jsonSerializer;

        public AuthorizationQueries(IHttpClientFactory httpClientFactory, DefaultJsonSerializer jsonSerializer)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
        }

        public async Task<bool> IsAuthorizedAsync(CancellationToken ct = default)
        {
            using var response = await _httpClientFactory.CreateClient("Metadata.API").GetAsync("authorization-status", ct);
            if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                if (_jsonSerializer.Deserialize<AuthorizationStatusDTO?>(content) is { } dto)
                {
                    return dto.IsAuthorized;
                }
            }
            return false;
        }

        public record AuthorizationStatusDTO(bool IsAuthorized);
    }
}