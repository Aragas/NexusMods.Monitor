﻿namespace NexusMods.Monitor.Subscriptions.API.Options
{
    public sealed record MetadataAPIOptions
    {
        public string APIEndpointV1 { get; init; } = default!;
    }
}