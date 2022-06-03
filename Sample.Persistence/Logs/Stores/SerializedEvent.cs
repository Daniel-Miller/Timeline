using System;
using Timeline.Events;

namespace Sample.Persistence.Logs.Stores
{
    /// <summary>
    /// Provides a serialization wrapper for events so that common properties are not embedded inside the event data.
    /// </summary>
    public class SerializedEvent : IEvent
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
        public SerializedEvent()
        {
            EventTime = DateTimeOffset.UtcNow;
        }
    }
}
