﻿using BetterHostedServices;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Bot.Slack.Application.CommandHandlers;
using NexusMods.Monitor.Bot.Slack.Application.IntegrationEventHandlers.Comments;
using NexusMods.Monitor.Bot.Slack.Application.Queries;
using NexusMods.Monitor.Bot.Slack.Host.BackgroundServices;
using NexusMods.Monitor.Bot.Slack.Host.Options;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Shared.Host.Extensions;

using NodaTime;

using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

using SlackNet.Bot;

using System;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Bot.Slack.Host
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

        private static IConfigurationRoot GetInitialConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddApplication();

                services.AddMediatR(typeof(SubscribeCommandHandler).Assembly);
                services.AddMemoryCache();
                services.AddHttpClient();
                services.AddHttpClient("Subscriptions.API", (sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<SubscriptionsAPIOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.APIEndpointV1);
                }).AddPolicyHandler(PollyUtils.PolicySelector);
                services.AddTransient<IClock, SystemClock>(_ => SystemClock.Instance);
                services.AddEventBusNatsAndEventHandlers(context.Configuration.GetSection("EventBus"), typeof(CommentAddedNewIntegrationEventHandler).Assembly);

                services.AddBetterHostedServices();

                services.Configure<SlackOptions>(context.Configuration.GetSection("Slack"));
                services.Configure<SubscriptionsAPIOptions>(context.Configuration.GetSection("Subscriptions"));

                services.AddSingleton<ISlackBot, SlackBotWrapper>();

                services.AddHostedServiceAsSingleton<SlackService>();

                services.AddTransient<ISubscriptionQueries, SubscriptionQueries>();
            })
            .ConfigureAppConfiguration((hostingContext, config) => config.AddEnvironmentVariables())
            .UseSerilog();
    }
}