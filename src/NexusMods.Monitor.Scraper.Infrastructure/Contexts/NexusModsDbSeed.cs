using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Domain.AggregatesModel.IssueAggregate;

using Polly;
using Polly.Retry;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Infrastructure.Contexts
{
    public class NexusModsDbSeed
    {
        public async Task SeedAsync(ILogger<NexusModsDbSeed> logger, NexusModsDb context)
        {
            var policy = CreatePolicy(logger, nameof(NexusModsDbSeed));

            await policy.ExecuteAsync(async () =>
            {
                await using var db = context;
                await context.Database.MigrateAsync();

                if (!context.IssuePriorityEnumerations.Any())
                {
                    await context.IssuePriorityEnumerations.AddRangeAsync(IssuePriorityEnumeration.List());
                }
                if (!context.IssueStatusEnumerations.Any())
                {
                    await context.IssueStatusEnumerations.AddRangeAsync(IssueStatusEnumeration.List());
                }

                await context.SaveChangesAsync();
            });
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<NexusModsDbSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<DbException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}