using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NexusMods.Monitor.Subscriptions.Application.Commands;
using NexusMods.Monitor.Subscriptions.Application.Queries;
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
            services.AddMediatR(typeof(SubscriptionAddCommand).Assembly);
            services.AddHttpClient();

            services.AddDbContext<SubscriptionDb>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Subscriptions")));

            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();

            services.AddTransient<ISubscriptionQueries, SubscriptionQueries>();

            services.AddControllers().AddNewtonsoftJson();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
