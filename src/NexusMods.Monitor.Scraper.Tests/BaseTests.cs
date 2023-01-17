using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using NodaTime;

using Xunit.Abstractions;

namespace NexusMods.Monitor.Scraper.Tests
{
    public abstract class BaseTests
    {
        protected readonly ITestOutputHelper OutputHelper;

        public BaseTests(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
        }

        protected virtual ServiceCollection GetServiceCollection()
        {
            var services = new ServiceCollection();
            services.AddLogging(buider => buider.AddXUnit(OutputHelper));
            services.AddSingleton<IClock>(SystemClock.Instance);
            return services;
        }
    }
}