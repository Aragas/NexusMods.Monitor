using CorrelationId;
using CorrelationId.HttpClient;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Metadata.API.Options;
using NexusMods.Monitor.Metadata.API.RateLimits;
using NexusMods.Monitor.Metadata.API.Services;
using NexusMods.Monitor.Metadata.Application.Queries.Comments;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Issues;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;
using NexusMods.Monitor.Metadata.Application.Queries.Threads;
using NexusMods.Monitor.Shared.API.Extensions;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Host;

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace NexusMods.Monitor.Metadata.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddAPI();

            services.AddMemoryCache();

            var assemblyName = Assembly.GetEntryAssembly()!.GetName();
            var userAgent = $"{assemblyName.Name} v{assemblyName.Version} ({Environment.OSVersion}; {RuntimeInformation.OSArchitecture}) {RuntimeInformation.FrameworkDescription}";
            services.AddHttpClient("NexusMods", (sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<NexusModsOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.Endpoint);
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);

                    var correlationIdOptions = sp.GetRequiredService<IOptions<CorrelationIdOptions>>().Value;
                    client.DefaultRequestHeaders.Add(correlationIdOptions.RequestHeader, Guid.NewGuid().ToString());
                })
                .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<SiteRateLimitHttpMessageHandler>())
                .AddPolicyHandler(PollyUtils.PolicySelector)
                .AddCorrelationIdOverrideForwarding()
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
            services.AddHttpClient("NexusMods.API", (sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<NexusModsOptions>>().Value;
                    var apiKeyProvider = sp.GetRequiredService<NexusModsAPIKeyProvider>();
                    client.BaseAddress = new Uri(backendOptions.APIEndpoint);
                    client.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    client.DefaultRequestHeaders.Add("apikey", apiKeyProvider.Get());
                    client.Timeout = TimeSpan.FromHours(1);

                    var correlationIdOptions = sp.GetRequiredService<IOptions<CorrelationIdOptions>>().Value;
                    client.DefaultRequestHeaders.Add(correlationIdOptions.RequestHeader, Guid.NewGuid().ToString());
                })
                .ConfigurePrimaryHttpMessageHandler(sp => sp.GetRequiredService<APIRateLimitHttpMessageHandler>())
                .AddPolicyHandler(PollyUtils.PolicySelector)
                .AddCorrelationIdOverrideForwarding()
                .SetHandlerLifetime(Timeout.InfiniteTimeSpan);

            services.Configure<NexusModsOptions>(Configuration.GetSection("NexusMods"));
            services.AddSingleton<NexusModsAPIKeyProvider>();

            services.AddSingleton<SiteRateLimitHttpMessageHandler>();
            services.AddSingleton<APIRateLimitHttpMessageHandler>();

            services.AddTransient<IIssueQueries, IssueQueries>();
            services.AddTransient<ICommentQueries, CommentQueries>();
            services.AddTransient<IGameQueries, GameQueries>();
            services.AddTransient<IModQueries, ModQueries>();
            services.AddTransient<IThreadQueries, ThreadQueries>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAPI();
        }
    }
}