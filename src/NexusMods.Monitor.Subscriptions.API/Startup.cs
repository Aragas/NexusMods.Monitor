using MediatR;

using MicroElements.Swashbuckle.NodaTime;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

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

using NodaTime;
using NodaTime.Serialization.SystemTextJson;

using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            services.AddMediatR(typeof(SubscriptionAddCommand).Assembly);

            var assemblyName = Assembly.GetEntryAssembly()?.GetName();
            var userAgent = $"{assemblyName?.Name ?? "NexusMods.Monitor.Subscriptions"} v{Assembly.GetEntryAssembly()?.GetName().Version}";
            services.AddHttpClient("Metadata.API", (sp, client) =>
            {
                var backendOptions = sp.GetRequiredService<IOptions<MetadataAPIOptions>>().Value;
                client.BaseAddress = new Uri(backendOptions.APIEndpointV1);
                client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            }).AddPolicyHandler(PollyUtils.PolicySelector);

            services.Configure<MetadataAPIOptions>(Configuration.GetSection("MetadataAPI"));

            services.AddDbContext<SubscriptionDb>(opt => opt.UseNpgsql(Configuration.GetConnectionString("Subscriptions"), o => o.UseNodaTime()));

            services.AddTransient<ISubscriptionRepository, SubscriptionRepository>();

            services.AddTransient<ISubscriptionQueries, SubscriptionQueries>();
            services.AddTransient<INexusModsGameQueries, NexusModsGameQueries>();
            services.AddTransient<INexusModsModQueries, NexusModsModQueries>();

            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
                options.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
            });
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "NexusMods.Monitor.Subscriptions.API", Version = "v1" });
                options.SupportNonNullableReferenceTypes();
                options.ConfigureForNodaTimeWithSystemTextJson();
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "NexusMods.Monitor.Subscriptions.API v1"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}