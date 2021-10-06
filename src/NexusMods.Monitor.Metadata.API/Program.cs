using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Metadata.API.Options;
using NexusMods.Monitor.Metadata.API.RateLimits;
using NexusMods.Monitor.Metadata.API.Services;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Shared.Host.Extensions;

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Metadata.API
{
    public static class Program
    {
        public static async Task Main(string[] args) => await new HostManager(CreateHostBuilder).StartAsync(args);

        public static IHostBuilder CreateHostBuilder(string[] args) => Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
            .AddNexusModsHttpClients();

        private static IHostBuilder AddNexusModsHttpClients(this IHostBuilder builder) => builder.ConfigureServices((context, services) =>
        {
            var assemblyName = Assembly.GetEntryAssembly()!.GetName();
            var userAgent = $"{assemblyName.Name} v{assemblyName.Version} ({Environment.OSVersion}; {RuntimeInformation.OSArchitecture}) {RuntimeInformation.FrameworkDescription}";
            services.AddHttpClient("NexusMods")
                .ConfigureHttpClient((sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<NexusModsOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.Endpoint);
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                })
                .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<SiteRateLimitHttpMessageHandler>())
                .GenerateCorrelationId()
                .AddPolly()
                .AddCorrelationIdOverrideForwarding()
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            services.AddHttpClient("NexusMods.Forum")
                .ConfigureHttpClient((sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<NexusModsOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.ForumEndpoint);
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                })
                .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<ForumRateLimitHttpMessageHandler>())
                .GenerateCorrelationId()
                .AddPolly()
                .AddCorrelationIdOverrideForwarding()
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            services.AddHttpClient("NexusMods.API")
                .ConfigureHttpClient((sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<NexusModsOptions>>().Value;
                    var apiKeyProvider = sp.GetRequiredService<NexusModsAPIKeyProvider>();
                    client.BaseAddress = new Uri(backendOptions.APIEndpoint);
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    client.DefaultRequestHeaders.Add("apikey", apiKeyProvider.Get());
                })
                .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<APIRateLimitHttpMessageHandler>())
                .GenerateCorrelationId()
                .AddPolly()
                .AddCorrelationIdOverrideForwarding()
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.AddValidatedOptions<NexusModsOptions, NexusModsOptionsValidator>(context.Configuration.GetSection("NexusMods"));

            services.AddSingleton<NexusModsAPIKeyProvider>();

            services.AddSingleton<SiteRateLimitHttpMessageHandler>();
            services.AddSingleton<ForumRateLimitHttpMessageHandler>();
            services.AddSingleton<APIRateLimitHttpMessageHandler>();
        });
    }
}