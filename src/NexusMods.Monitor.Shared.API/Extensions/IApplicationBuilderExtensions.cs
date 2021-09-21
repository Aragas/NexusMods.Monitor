using CorrelationId;

using Microsoft.AspNetCore.Builder;

using System;
using System.Reflection;

namespace NexusMods.Monitor.Shared.API.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAPI(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            var appName = Assembly.GetEntryAssembly()!.GetName().Name;

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", appName));

            app.UseCorrelationId();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}