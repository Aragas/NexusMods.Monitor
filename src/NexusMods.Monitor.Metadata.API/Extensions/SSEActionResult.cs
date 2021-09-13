using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.API.Extensions
{
    public class SSEActionResult : IActionResult
    {
        private readonly IAsyncEnumerable<SSEMessage> _messages;

        public SSEActionResult(IAsyncEnumerable<SSEMessage> messages) => _messages = messages;

        public async Task ExecuteResultAsync(ActionContext context)
        {
            await SSEInitAsync(context.HttpContext);

            await foreach (var @event in _messages)
            {
                await SSESendEventAsync(context.HttpContext, @event);
            }
        }


        private static async Task SSEInitAsync(HttpContext ctx)
        {
            ctx.Response.StatusCode = 200;
            ctx.Response.Headers.Add("Cache-Control", "no-cache");
            ctx.Response.Headers.Add("Content-Type", "text/event-stream");
            ctx.Response.Headers.Add("Connection", "keep-alive");
            await ctx.Response.Body.FlushAsync();
        }

        private static async Task SSESendEventAsync(HttpContext ctx, SSEMessage @event)
        {
            if (@event.IsEmpty) return;

            if (!string.IsNullOrWhiteSpace(@event.Id))
                await ctx.Response.WriteAsync($"id: {@event.Id}\n");

            if (@event.Retry is not null)
                await ctx.Response.WriteAsync($"retry: {@event.Retry}\n");

            if (!string.IsNullOrWhiteSpace(@event.Event))
                await ctx.Response.WriteAsync($"event: {@event.Event}\n");

            await foreach (var line in @event.Data ?? AsyncEnumerable.Empty<string>())
                await ctx.Response.WriteAsync($"data: {line}\n");

            await ctx.Response.WriteAsync("\n");
            await ctx.Response.Body.FlushAsync();
        }
    }
}