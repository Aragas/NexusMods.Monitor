using Microsoft.Extensions.Hosting;

using NexusMods.Monitor.Shared.Common.Extensions;
using NexusMods.Monitor.Shared.Host.Extensions;

using Serilog;

using System;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Host
{
    public sealed class HostManager
    {
        private readonly ILogger _logger;
        private readonly Func<string[], IHostBuilder> _factory;

        public HostManager(Func<string[], IHostBuilder> factory)
        {
            _logger = HostExtensions.BuildSerilogLogger().CreateGlobalLogger();
            _factory = factory;
        }

        public async ValueTask StartAsync(string[] args)
        {
            try
            {
                _logger.Warning("Starting.");

                var hostBuilder = _factory(args);

                var host = hostBuilder.Build();

                await host.RunAsync();
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Fatal exception.");
                throw;
            }
            finally
            {
                _logger.Warning("Stopped.");
                await _logger.TryDisposeAsync();
            }
        }
    }
}