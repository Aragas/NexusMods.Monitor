using Enbiso.NLib.EventBus;
using Enbiso.NLib.EventBus.Nats;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NATS.Client;

using NexusMods.Monitor.Scraper.Application.Extensions;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Host.Options;

using Serilog;
using Serilog.Exceptions;
using Serilog.Exceptions.Core;
using Serilog.Exceptions.EntityFrameworkCore.Destructurers;

using System;
using System.Linq;
using System.Reflection;

namespace NexusMods.Monitor.Shared.Host.Extensions
{
    public static class HostExtensions
    {
        public static bool IsInKubernetes(this IHost host)
        {
            var cfg = host.Services.GetRequiredService<IConfiguration>();
            var orchestratorType = cfg["OrchestratorType"];
            return orchestratorType?.ToUpper() == "K8S";
        }

        public static ILogger CreateGlobalLogger(this LoggerConfiguration loggerConfiguration) => Log.Logger = loggerConfiguration.CreateLogger();

        public static IConfigurationRoot GetInitialConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env}.json", true)
                .AddEnvironmentVariables()
                .Build();
        }

        public static LoggerConfiguration BuildSerilogLogger()
        {
            var configuration = GetInitialConfiguration();
            return new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithAssemblyName()
                .Enrich.WithAssemblyVersion()
                .Enrich.WithThreadId()
                .Enrich.WithThreadName()
                .Enrich.WithExceptionDetails(new DestructuringOptionsBuilder()
                    .WithDefaultDestructurers()
                    .WithDestructurers(new[] { new DbUpdateExceptionDestructurer() }))
                .ReadFrom.Configuration(configuration);
        }

        public static IHostBuilder AddSubscriptionsHttpClient(this IHostBuilder builder) => builder.ConfigureServices((context, services) =>
        {
            services.AddHttpClient("Subscriptions.API")
                .ConfigureHttpClient((sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<SubscriptionsAPIOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.APIEndpointV1);
                })
                .GenerateCorrelationId()
                .AddPolly()
                .AddCorrelationIdOverrideForwarding();

            services.AddOptions<SubscriptionsAPIOptions>()
                .Bind(context.Configuration.GetSection("SubscriptionsAPI"))
                .ValidateViaFluent<SubscriptionsAPIOptions, SubscriptionsAPIOptionsValidator>()
                .ValidateOnStart();
        });

        public static IHostBuilder AddMetadataHttpClient(this IHostBuilder builder) => builder.ConfigureServices((context, services) =>
        {
            services.AddHttpClient("Metadata.API")
                .ConfigureHttpClient((sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<MetadataAPIOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.APIEndpointV1);
                })
                .GenerateCorrelationId()
                .AddPolly()
                .AddCorrelationIdOverrideForwarding();

            services.AddOptions<MetadataAPIOptions>()
                .Bind(context.Configuration.GetSection("MetadataAPI"))
                .ValidateViaFluent<MetadataAPIOptions, MetadataAPIOptionsValidator>()
                .ValidateOnStart();
        });

        public static IHostBuilder AddEventBusNatsAndEventHandlers(this IHostBuilder builder, Assembly assembly) => builder.ConfigureServices((context, services) =>
        {
            services.AddOptions<NatsOptions>()
                .Bind(context.Configuration.GetSection("EventBus"))
                .ValidateViaFluent<NatsOptions, NatsOptionsValidator>()
                .ValidateOnStart();

            services.AddEventBus();
            services.AddSingleton<ConnectionFactory>();
            services.AddSingleton<INatsConnection, NatsConnection>();
            services.AddSingleton<IEventPublisher, NatsEventPublisher>();
            services.AddSingleton<IEventSubscriber, NatsEventSubscriber>();

            services.Replace(new ServiceDescriptor(typeof(IEventProcessor), typeof(EventProcessorJson), ServiceLifetime.Singleton));
            foreach (var type in assembly.GetTypes().Where(typeof(IEventHandler).IsAssignableFrom))
            {
                services.AddTransient(typeof(IEventHandler), type);
            }
        });
    }
}