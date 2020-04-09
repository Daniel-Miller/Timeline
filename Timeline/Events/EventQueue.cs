using System;
using System.Collections.Generic;
using System.Linq;

using Timeline.Exceptions;

namespace Timeline.Events
{
    /// <summary>
    /// Implements a basic event queue.
    /// </summary>
    public class EventQueue : IEventQueue
    {
        /// <summary>
        /// An event's full class name is used as the key to a list of event-handling methods.
        /// </summary>
        readonly Dictionary<string, List<Action<IEvent>>> _subscribers;

        /// <summary>
        /// In a multi-tenant system we may want to allow each individual tenant to override/customize the handling of 
        /// an event. The class name and the tenant identifier is used as the unique key here.
        /// </summary>
        readonly Dictionary<(string, Guid), Action<IEvent>> _overriders;

        /// <summary>
        /// Constructs the queue.
        /// </summary>
        public EventQueue()
        {
            _subscribers = new Dictionary<string, List<Action<IEvent>>>();
            _overriders = new Dictionary<(string, Guid), Action<IEvent>>();
        }

        /// <summary>
        /// Invokes each subscriber method registered to handle the event.
        /// </summary>
        /// <param name="event"></param>
        public void Publish(IEvent @event)
        {
            var name = @event.GetType().FullName;

            if (_overriders.ContainsKey((name, @event.IdentityTenant)))
            {
                var customization = _overriders[(name, @event.IdentityTenant)];
                if (customization != null)
                    customization.Invoke(@event);
            }
            else if (_subscribers.ContainsKey(name))
            {
                var actions = _subscribers[name];
                foreach (var action in actions)
                    action.Invoke(@event);
            }
            else
            {
                throw new UnhandledEventException(name);
            }
        }

        /// <summary>
        /// Any number of subscribers can register for an event, and any one subscriber can register any number of
        /// methods to be invoked when the event is published. 
        /// </summary>
        public void Subscribe<T>(Action<T> action) where T : IEvent
        {
            var name = typeof(T).FullName;

            if (!_subscribers.Any(x => x.Key == name))
                _subscribers.Add(name, new List<Action<IEvent>>());

            _subscribers[name].Add((@event) => action((T)@event));
        }

        /// <summary>
        /// Register a custom tenant-specific handler for the event.
        /// </summary>
        public void Override<T>(Action<T> action, Guid tenant) where T : IEvent
        {
            var name = typeof(T).AssemblyQualifiedName;

            if (_overriders.Any(x => x.Key.Item1 == name && x.Key.Item2 == tenant))
                throw new AmbiguousCommandHandlerException(name);

            _overriders.Add((name, tenant), (command) => action((T)command));
        }
    }
}
