using CorrelationId.DependencyInjection;

using MicroElements.Swashbuckle.NodaTime;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using NodaTime;
using NodaTime.Serialization.SystemTextJson;

using System;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NexusMods.Monitor.Shared.API.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddAPI(this IServiceCollection services)
        {
            var appName = Assembly.GetEntryAssembly()!.GetName().Name;

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

                var xmlFile = $"{appName}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            return services;
        }
    }
}