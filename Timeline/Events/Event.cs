using System;

namespace Timeline.Events
{
    /// <summary>
    /// Defines the base class for all events.
    /// </summary>
    /// <remarks>
    /// An event represents something that has taken place in the domain. It is always named with a past-participle 
    /// verb, such as Order Confirmed. Since an event represents something in the past, it can be considered a 
    /// statement of fact, which can be used to make decisions in other parts of the system.Events are immutable 
    /// because they represent domain actions that occurred in the past, and the past cannot be altered.
    /// </remarks>
    public class Event : IEvent
    {
        /// <summary>
        /// Identifies the aggregate for which the event was raised.
        /// </summary>
        public Guid AggregateIdentifier { get; set; }

        /// <summary>
        /// Version number of the aggregate for which the event was raised.
        /// </summary>
        public int AggregateVersion { get; set; }

        /// <summary>
        /// Identifies the tenant for the session in which the event was raised.
        /// </summary>
        public Guid IdentityTenant { get; set; }

        /// <summary>
        /// Identifies the user for the session in which the event was raised.
        /// </summary>
        public Guid IdentityUser { get; set; }

        /// <summary>
        /// Fully-qualified assembly name for the class that implements the event.
        /// </summary>
        public string EventClass { get; set; }

        /// <summary>
        /// Abbreviated class name.
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Serialized data for the event.
        /// </summary>
        public string EventData { get; set; }

        /// <summary>
        /// Date and time the event was raised.
        /// </summary>
        public DateTimeOffset EventTime { get; set; }

        /// <summary>
        /// Constructs a new instance. By default the event time is now.
        /// </summary>
        public Event()
        {
            EventTime = DateTimeOffset.UtcNow;
        }
    }
}
