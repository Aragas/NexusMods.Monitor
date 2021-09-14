using CorrelationId;

using Microsoft.AspNetCore.Builder;

using System.Reflection;

namespace NexusMods.Monitor.Shared.API.Extensions
{
    public static class IApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAPI(this IApplicationBuilder app)
        {
            var appName = Assembly.GetEntryAssembly()!.GetName().Name;

            app.UseCorrelationId();

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", appName));

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            return app;
        }
    }
}
