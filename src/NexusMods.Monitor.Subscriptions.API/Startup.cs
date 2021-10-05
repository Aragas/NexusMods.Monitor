using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NexusMods.Monitor.Shared.API.Extensions;
using NexusMods.Monitor.Shared.Application.Extensions;
using NexusMods.Monitor.Shared.Infrastructure.Npgsql.Extensions;
using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsGames;
using NexusMods.Monitor.Subscriptions.Application.Queries.NexusModsMods;
using NexusMods.Monitor.Subscriptions.Application.Queries.Subscriptions;
using NexusMods.Monitor.Subscriptions.Domain.AggregatesModel.SubscriptionAggregate;
using NexusMods.Monitor.Subscriptions.Infrastructure.Contexts;
using NexusMods.Monitor.Subscriptions.Infrastructure.Repositories;

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

            services.AddDbContext<SubscriptionDb>(opt => opt.UseNpgsql2(Configuration.GetConnectionString("Subscriptions")));

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