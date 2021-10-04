using Community.Microsoft.Extensions.Caching.PostgreSql;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NexusMods.Monitor.Metadata.Application.Queries.Comments;
using NexusMods.Monitor.Metadata.Application.Queries.Games;
using NexusMods.Monitor.Metadata.Application.Queries.Issues;
using NexusMods.Monitor.Metadata.Application.Queries.Mods;
using NexusMods.Monitor.Metadata.Application.Queries.Threads;
using NexusMods.Monitor.Shared.API.Extensions;
using NexusMods.Monitor.Shared.Application.Extensions;

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

            services.AddDistributedPostgreSqlCache(o =>
            {
                o.ConnectionString = Configuration.GetConnectionString("Cache");
                o.SchemaName = "cache";
                o.TableName = "distributed_cache";
            });

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