using CorrelationId;
using CorrelationId.HttpClient;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using NexusMods.Monitor.Shared.API.Extensions;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Host;
using NexusMods.Monitor.Subscriptions.API.Options;
using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsGames;
using NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsMods;
using NexusMods.Monitor.Subscriptions.Application.Queries.Subscriptions;
using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;
using NexusMods.Monitor.Subscriptions.Infrastructure.Repositories;

using System;

namespace NexusMods.Monitor.Subscriptions.API
{
    public sealed class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication();
            services.AddAPI();

            services.AddMediatR(typeof(SubscriptionAddCommand).Assembly);

            services.AddHttpClient("Metadata.API", (sp, client) =>
                {
                    var backendOptions = sp.GetRequiredService<IOptions<MetadataAPIOptions>>().Value;
                    client.BaseAddress = new Uri(backendOptions.APIEndpointV1);

                    var correlationIdOptions = sp.GetRequiredService<IOptions<CorrelationIdOptions>>().Value;
                    client.DefaultRequestHeaders.Add(correlationIdOptions.RequestHeader, Guid.NewGuid().ToString());
                })
                .AddPolicyHandler(PollyUtils.PolicySelector)
                .AddCorrelationIdOverrideForwarding();

            services.Configure<MetadataAPIOptions>(Configuration.GetSection("MetadataAPI"));

            services.AddDbContext<SubscriptionDb>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Subscriptions"), o => o.UseNodaTime()));

            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();

            services.AddTransient<ISubscriptionQueries, SubscriptionQueries>();
            services.AddTransient<INexusModsGameQueries, NexusModsGameQueries>();
            services.AddTransient<INexusModsModQueries, NexusModsModQueries>();

        }

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