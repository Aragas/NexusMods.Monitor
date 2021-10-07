using CorrelationId;
using CorrelationId.DependencyInjection;

using FluentValidation;

using MediatR;
using MediatR.Pipeline;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

using NexusMods.Monitor.Shared.Application.Behaviours;
using NexusMods.Monitor.Shared.Application.HttpLogging;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Reflection;

namespace NexusMods.Monitor.Shared.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddDefaultCorrelationId(options =>
            {
                options.AddToLoggingScope = true;
                options.EnforceHeader = true;
                options.IgnoreRequestHeader = false;
                options.IncludeInResponse = true;
                options.RequestHeader = CorrelationIdOptions.DefaultHeader;
                options.ResponseHeader = CorrelationIdOptions.DefaultHeader;
                options.UpdateTraceIdentifier = false;
            });

            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehaviour<>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));

            services.AddSingleton<DefaultJsonSerializer>();

            services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
            services.AddSingleton<IHttpMessageHandlerBuilderFilter, LoggingHttpMessageHandlerBuilderFilter>();
            return services;
        }
    }
}