using CorrelationId.DependencyInjection;

using FluentValidation;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using NexusMods.Monitor.Shared.Application.Behaviours;
using NexusMods.Monitor.Shared.Common;

using System.Reflection;

namespace NexusMods.Monitor.Shared.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddDefaultCorrelationId(options =>
            {
                options.AddToLoggingScope = true;
                options.EnforceHeader = true;
                options.IgnoreRequestHeader = false;
                options.IncludeInResponse = true;
                options.RequestHeader = "X-Correlation-Id";
                options.ResponseHeader = "X-Correlation-Id";
                options.UpdateTraceIdentifier = false;
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            services.AddSingleton<DefaultJsonSerializer>();

            return services;
        }
    }
}