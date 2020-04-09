using System;

namespace Timeline.Events
{
    /// <summary>
    /// Provides the features for a basic service bus to handle the publication of events.
    /// </summary>
    public interface IEventQueue
    {
        /// <summary>
        /// Publishes an event to registered subscribers.
        /// </summary>
        void Publish(IEvent @event);

        /// <summary>
        /// Registers a handler for a specific event.
        /// </summary>
        void Subscribe<T>(Action<T> action) where T : IEvent;

        /// <summary>
        /// Register a custom tenant-specific handler for the event. 
        /// </summary>
        void Override<T>(Action<T> action, Guid tenant) where T : IEvent;
    }
}
