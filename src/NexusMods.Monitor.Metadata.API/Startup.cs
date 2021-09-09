﻿using MicroElements.Swashbuckle.NodaTime;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using NexusMods.Monitor.Metadata.API.Options;
using NexusMods.Monitor.Metadata.Application.Queries.Comments;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Issues;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;
using NexusMods.Monitor.Metadata.Application.Queries.Threads;

using NexusModsNET;

using NodaTime;
using NodaTime.Serialization.SystemTextJson;

using System.Text.Json;
using System.Text.Json.Serialization;

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
            services.AddMemoryCache();
            services.AddHttpClient();

            services.Configure<NexusModsOptions>(Configuration.GetSection("NexusMods"));

            services.AddTransient<INexusModsClient, NexusModsClientWrapper>();

            services.AddTransient<IIssueQueries, IssueQueries>();
            services.AddTransient<ICommentQueries, CommentQueries>();
            services.AddTransient<IGameQueries, GameQueries>();
            services.AddTransient<IModQueries, ModQueries>();
            services.AddTransient<IThreadQueries, ThreadQueries>();

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
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "NexusMods.Monitor.Metadata.API", Version = "v1" });
                options.SupportNonNullableReferenceTypes();
                options.ConfigureForNodaTimeWithSystemTextJson();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "NexusMods.Monitor.Metadata.API v1"));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}