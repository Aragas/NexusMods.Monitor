using Enbiso.NLib.EventBus;

using NexusMods.Monitor.Shared.Application;
using NexusMods.Monitor.Shared.Common;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NexusMods.Monitor.Shared.Host
{
    internal class EventProcessorJson : IEventProcessor
    {
        private readonly Dictionary<string, List<IEventHandler>> _subscriptions = new();

        private readonly IEnumerable<IEventHandler> _eventHandlers;

        private readonly DefaultJsonSerializer _jsonSerializer;

        public EventProcessorJson(DefaultJsonSerializer jsonSerializer, IEnumerable<IEventHandler> eventHandlers)
        {
            _jsonSerializer = jsonSerializer;
            _eventHandlers = eventHandlers;
        }

        public async Task ProcessEvent(string eventName, byte[] data)
        {
            var message = Encoding.UTF8.GetString(data);

            if (!_subscriptions.ContainsKey(eventName)) return;

            foreach (var eventHandler in _subscriptions[eventName])
            {
                var eventType = eventHandler.GetEventType();
                var @event = _jsonSerializer.Deserialize(message, eventType);
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