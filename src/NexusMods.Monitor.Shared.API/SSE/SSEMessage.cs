﻿using System.Collections.Generic;

namespace NexusMods.Monitor.Shared.API.SSE
{
    public record SSEMessage(string? Id = null, string? Event = null, IAsyncEnumerable<string>? Data = null, int? Retry = null)
    {
        public bool IsEmpty => Id is null && Event is null && Data is null;
    }
}