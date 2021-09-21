using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Shared.Host.Extensions;
using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;

using Polly;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.API
{
    public class Program
    {
        public static async Task Main(string[] args) => await new HostManager(CreateHostBuilder)
            .ExecuteBeforeRun(async host =>
            {
                await EnsureDatabasesCreated(host);
            })
            .StartAsync(args);

        private static async Task EnsureDatabasesCreated(IHost host)
        {
            var retryPolicy = Policy.Handle<Exception>(ex => ex.GetType() != typeof(TaskCanceledException))
                .WaitAndRetryAsync(10, retryAttempt => TimeSpan.FromSeconds(2),
                    (ex, time) =>
                    {
                        using var scope = host.Services.CreateScope();
                        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "Exception during PostgreSQL connection. Waiting {time}...", time);
                    });

            await retryPolicy.ExecuteAsync(async token =>
            {
                using var scope = host.Services.CreateScope();
                await using var subscriptionDb = scope.ServiceProvider.GetRequiredService<SubscriptionDb>();
                //await subscriptionDb.Database.EnsureDeletedAsync(token);
                await subscriptionDb.Database.EnsureCreatedAsync(token);
            }, CancellationToken.None);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .AddMetadataHttpClient()
            //.AddNpgsqlConnection<SubscriptionDb>("Subscriptions")
        ;
    }
}