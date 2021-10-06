using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Polly;
using Polly.Retry;

using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts
{
    public sealed class NexusModsDbSeed
    {
        public static async Task SeedAsync(ILogger<NexusModsDbSeed> logger, NexusModsDb context)
        {
            var policy = CreatePolicy(logger, nameof(NexusModsDbSeed));

            await policy.ExecuteAsync(async () =>
            {
                await using var db = context;
                await context.Database.MigrateAsync();
            });
        }

        private static AsyncRetryPolicy CreatePolicy(ILogger<NexusModsDbSeed> logger, string prefix, int retries = 3) => Policy.Handle<DbException>()
            .WaitAndRetryAsync(
                retryCount: retries,
                sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                onRetry: (exception, timeSpan, retry, ctx) =>
                {
                    logger.LogWarning(exception,
                        "[{Prefix}] Exception {ExceptionType} with message {Message} detected on attempt {Retry} of {Retries}",
                        prefix, exception.GetType().Name, exception.Message, retry, retries);
                }
            );
    }
}