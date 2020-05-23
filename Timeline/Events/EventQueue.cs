using System;
using System.Collections.Generic;
using System.Linq;

using Timeline.Exceptions;
using Timeline.Utilities;

namespace Timeline.Events
{
    /// <summary>
    /// Implements a basic event queue.
    /// </summary>
    public class EventQueue : IEventQueue
    {
        readonly ISerializer _serializer;
        
        /// <summary>
        /// An event's full class name is used as the key to a list of event-handling methods.
        /// </summary>
        readonly Dictionary<string, List<Action<IEvent>>> _subscribers;

        /// <summary>
        /// In a multi-tenant system we may want to allow each individual tenant to override/customize the handling of 
        /// an event. The class name and the tenant identifier is used as the unique key here.
        /// </summary>
        readonly Dictionary<EventOverrideKey, Action<IEvent>> _overriders;

        /// <summary>
        /// Constructs the queue.
        /// </summary>
        public EventQueue(ISerializer serializer)
        {
            _serializer = serializer;
            _subscribers = new Dictionary<string, List<Action<IEvent>>>();
            _overriders = new Dictionary<EventOverrideKey, Action<IEvent>>();
        }

        /// <summary>
        /// Invokes each subscriber method registered to handle the event.
        /// </summary>
        /// <param name="event"></param>
        public void Publish(IEvent @event)
        {
            var eventName = _serializer.GetClassName(@event.GetType());

            var key = new EventOverrideKey
            {
                EventName = eventName,
                IdentityTenant = @event.IdentityTenant
            };

            if (_overriders.Keys.Any(k => k.EventName == key.EventName && k.IdentityTenant == @event.IdentityTenant))
            {
                var customization = _overriders
                    .FirstOrDefault(kv => kv.Key.EventName == key.EventName && kv.Key.IdentityTenant == @event.IdentityTenant)
                    .Value;
                customization?.Invoke(@event);
            }
            else if (_subscribers.ContainsKey(eventName))
            {
                var actions = _subscribers[eventName];
                foreach (var action in actions)
                    action.Invoke(@event);
            }
            else
            {
                throw new UnhandledEventException(eventName);
            }
        }

        /// <summary>
        /// Any number of subscribers can register for an event, and any one subscriber can register any number of
        /// methods to be invoked when the event is published. 
        /// </summary>
        public void Subscribe<T>(Action<T> action) where T : IEvent
        {
            var name = _serializer.GetClassName(typeof(T));

            if (!_subscribers.Any(x => x.Key == name))
                _subscribers.Add(name, new List<Action<IEvent>>());

            _subscribers[name].Add((@event) => action((T)@event));
        }

        /// <summary>
        /// Register a custom tenant-specific handler for the event.
        /// </summary>
        public void Override<T>(Action<T> action, Guid tenant) where T : IEvent
        {
            var key = new EventOverrideKey
            {
                EventName = typeof(T).FullName,
                IdentityTenant = tenant
            };

            if (_overriders.Any(x => x.Key.EventName == key.EventName && x.Key.IdentityTenant == key.IdentityTenant))
                throw new AmbiguousCommandHandlerException(key.EventName);

            _overriders.Add(key, (command) => action((T)command));
        }
    }
}
