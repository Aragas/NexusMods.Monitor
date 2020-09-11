using Enbiso.NLib.EventBus;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Host
{
    public class EventProcessorNewtonsoftJson : IEventProcessor
    {
        private readonly Dictionary<string, List<IEventHandler>> _subscriptions = new Dictionary<string, List<IEventHandler>>();

        private readonly IEnumerable<IEventHandler> _eventHandlers;

        public EventProcessorNewtonsoftJson(IEnumerable<IEventHandler> eventHandlers)
        {
            _eventHandlers = eventHandlers;
        }

        public async Task ProcessEvent(string eventName, byte[] data)
        {
            var message = Encoding.UTF8.GetString(data);

            if (!_subscriptions.ContainsKey(eventName)) return;

            foreach (var eventHandler in _subscriptions[eventName])
            {
                var eventType = eventHandler.GetEventType();
                var @event = JsonConvert.DeserializeObject(message, eventType);
                await eventHandler.Handle(@event);
            }
        }

        public void Setup(Action<string> onAddSubscription)
        {
            foreach (var eventHandler in _eventHandlers)
            {
                var eventName = eventHandler.GetEventType().Name;

                onAddSubscription?.Invoke(eventName);

                if (_subscriptions.TryGetValue(eventName, out var currentHandlers))
                {
                    currentHandlers.Add(eventHandler);
                    _subscriptions[eventName] = currentHandlers;
                }
                else
                {
                    _subscriptions.Add(eventName, new List<IEventHandler> { eventHandler });
                }
            }
        }
    }
}