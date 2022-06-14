using System;
using System.Collections.Generic;

using Timeline.Utilities;

namespace Timeline.Events
{
    /// <summary>
    /// Defines the methods needed from the event store.
    /// </summary>
    public interface IEventStore
    {
        /// <summary>
        /// Returns true if an aggregate exists.
        /// </summary>
        bool Exists(Guid aggregate);

        /// <summary>
        /// Returns true if an aggregate with a specific version exists.
        /// </summary>
        bool Exists(Guid aggregate, int version);

        /// <summary>
        /// Gets events for an aggregate starting at a specific version. To get all events use version = -1.
        /// </summary>
        IEnumerable<IEvent> Get(Guid aggregate, int version);

        /// <summary>
        /// Gets all aggregates that are scheduled to expire at (or before) a specific time on a specific date.
        /// </summary>
        IEnumerable<Guid> GetExpired(DateTimeOffset at);

        /// <summary>
        /// Save events.
        /// </summary>
        void Save(IAggregateRoot aggregate, IEnumerable<IEvent> events);

        /// <summary>
        /// Copies an aggregate to offline storage and removes it from online logs.
        /// </summary>
        /// <remarks>
        /// Someone who is a purist with regard to event sourcing will red-flag this function and say the event stream 
        /// for an aggregate should never be altered or removed. However, we have two scenarios in which this is a non-
        /// negotiable business requirement. First, when a customer does not renew their contract with our business, we
        /// have a contractual obligation to remove all the customer's data from our systems. Second, we frequently run
        /// test-cases to confirm system functions are operating correctly; this data is temporary by definition, and 
        /// we do not want to permanently store the event streams for test aggregates.
        /// </remarks>
        void Box(Guid aggregate);
    }
}
