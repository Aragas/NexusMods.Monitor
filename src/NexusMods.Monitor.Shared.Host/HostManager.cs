using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Shared.Common.Extensions;
using NexusMods.Monitor.Shared.Host.Extensions;
using NexusMods.Monitor.Shared.Host.Options;

using Serilog;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Host
{
    public sealed class HostManager
    {
        private readonly ILogger _logger;
        private readonly Func<string[], IHostBuilder> _factory;
        private readonly List<Func<IHost, Task>> _beforeRun = new();

        public HostManager(Func<string[], IHostBuilder> factory)
        {
            var configuration = SetBaseConfiguration(new ConfigurationBuilder()).Build();
            _logger = configuration.BuildSerilogLogger().CreateGlobalLogger();
            _factory = factory;
        }

        public HostManager ExecuteBeforeRun(Func<IHost, Task> action)
        {
            _beforeRun.Add(action);
            return this;
        }

        public async ValueTask StartAsync(string[] args)
        {
            try
            {
                _logger.Warning("Starting");

                var hostBuilder = _factory(args)
                    .ConfigureHostConfiguration(builder => SetBaseConfiguration(builder))
                    .ConfigureAppConfiguration(config => config.AddEnvironmentVariables())
                    .UseSerilog();

                using var host = hostBuilder.Build();

                ValidateOptions(host);

                foreach (var func in _beforeRun)
                {
                    await func(host);
                }

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Fatal exception");
                throw;
            }
            finally
            {
                _logger.Warning("Stopped");
                await _logger.TryDisposeAsync();
            }
        }

        private static void ValidateOptions(IHost host)
        {
            var options = host.Services.GetRequiredService<IOptions<ValidatorOptions>>();

            var exceptions = new List<Exception>();

            foreach (var validate in options.Value?.Validators.Values ?? Enumerable.Empty<Action>())
            {
                try
                {
                    // Execute the validation method and catch the validation error
                    validate();
                }
                catch (OptionsValidationException ex)
                {
                    exceptions.Add(ex);
                }
            }

            if (exceptions.Count == 1)
            {
                // Rethrow if it's a single error
                ExceptionDispatchInfo.Capture(exceptions[0]).Throw();
            }

            if (exceptions.Count > 1)
            {
                // Aggregate if we have many errors
                throw new AggregateException(exceptions);
            }
        }

        private static IConfigurationBuilder SetBaseConfiguration(IConfigurationBuilder builder)
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
            return builder
                .AddJsonFile($"appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env}.json", true, true)
                .AddEnvironmentVariables();
        }
    }
}