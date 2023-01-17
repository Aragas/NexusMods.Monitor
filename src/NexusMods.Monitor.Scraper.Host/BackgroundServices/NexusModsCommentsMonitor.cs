using BetterHostedServices;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Scraper.Application;

using Polly;

using RateLimiter;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Scraper.Host.BackgroundServices
{
    public sealed class NexusModsCommentsMonitor : CriticalBackgroundService
    {
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeLimiter _timeLimiter;

        public NexusModsCommentsMonitor(ILogger<NexusModsCommentsMonitor> logger, IServiceScopeFactory scopeFactory, IApplicationEnder applicationEnder) : base(applicationEnder)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            _timeLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromSeconds(90));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var loggingScope = _logger.BeginScope("Service: {Service}", nameof(NexusModsCommentsMonitor));

            stoppingToken.Register(() => _logger.LogInformation("Comments processing is stopping"));

            var policy = Policy
                .Handle<Exception>(ex => ex.GetType() != typeof(TaskCanceledException))
                .WaitAndRetryForeverAsync(
                    retryAttempt => TimeSpan.FromMinutes(10),
                    (ex, time) => _logger.LogError(ex, "Exception during comments processing. Waiting {Time}...", time));

            while (!stoppingToken.IsCancellationRequested)
            {
                await policy.ExecuteAsync(async ct => await _timeLimiter.Enqueue(async () =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var processor = scope.ServiceProvider.GetRequiredService<NexusModsCommentsProcessor>();
                    await processor.Process(ct);
                }, ct), stoppingToken);
            }
        }
    }
}