using Enbiso.NLib.EventBus;
using Enbiso.NLib.EventBus.Nats;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using NATS.Client;

using System.Linq;
using System.Reflection;

namespace NexusMods.Monitor.Shared.Host.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddEventBusNatsAndEventHandlers(this IServiceCollection services, IConfigurationSection configurationSection, Assembly assembly)
        {
            services.Configure<NatsOptions>(configurationSection);

            services.AddEventBus();
            services.AddSingleton<ConnectionFactory>();
            services.AddSingleton<INatsConnection, NatsConnection>();
            services.AddSingleton<IEventPublisher, NatsEventPublisher>();
            services.AddSingleton<IEventSubscriber, NatsEventSubscriber>();

            services.Replace(new ServiceDescriptor(typeof(IEventProcessor), typeof(EventProcessorNewtonsoftJson), ServiceLifetime.Singleton));
            foreach (var type in assembly.GetTypes().Where(typeof(IEventHandler).IsAssignableFrom))
            {
                services.AddTransient(typeof(IEventHandler), type);
            }
            return services;
        }
    }
}