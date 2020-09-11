using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;

using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

using System;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Subscriptions.API
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = GetInitialConfiguration();
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Warning("Starting.");

                var hostBuilder = CreateHostBuilder(args);

                var host = hostBuilder.Build();
                await EnsureDatabasesCreated(host);

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal exception.");
                throw;
            }
            finally
            {
                Log.Warning("Stopped.");
                Log.CloseAndFlush();
            }
        }

        private static async Task EnsureDatabasesCreated(IHost host)
        {
            using var scope = host.Services.CreateScope();
            using var subscriptionDb = scope.ServiceProvider.GetRequiredService<SubscriptionDb>();
            //await subscriptionDb.Database.EnsureDeletedAsync();
            await subscriptionDb.Database.EnsureCreatedAsync();
        }

        private static IConfigurationRoot GetInitialConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .ConfigureAppConfiguration(config => config.AddEnvironmentVariables())
                .UseSerilog();
    }
}
