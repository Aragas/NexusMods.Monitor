using System;

namespace NexusMods.Monitor.Bot.Slack.Application.Queries.RateLimits
{
    public record RateLimitViewModel(APILimitViewModel APILimit, SiteLimitViewModel SiteLimit);
    public record APILimitViewModel(int HourlyLimit, int HourlyRemaining, DateTime HourlyReset, int DailyLimit, int DailyRemaining, DateTime DailyReset);
    public record SiteLimitViewModel(DateTimeOffset? RetryAfter);
}