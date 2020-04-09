using System;
using System.Linq;

using Timeline.Exceptions;

namespace Timeline.Events
{
    /// <summary>
    /// Saves and gets aggregates to and from an event store.
    /// </summary>
    public class EventRepository : IEventRepository
    {
        private readonly IEventStore _store;

        public EventRepository(IEventStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        /// <summary>
        /// Gets an aggregate from the event store.
        /// </summary>
        public T Get<T>(Guid aggregate) where T : AggregateRoot
        {
            return Rehydrate<T>(aggregate);
        }

        /// <summary>
        /// Saves all uncommitted changes to the event store.
        /// </summary>
        public IEvent[] Save<T>(T aggregate, int? version) where T : AggregateRoot
        {
            if (version != null && (_store.Exists(aggregate.AggregateIdentifier, version.Value)))
                throw new ConcurrencyException(aggregate.AggregateIdentifier);

            // Get the list of events that are not yet saved. 
            var events = aggregate.FlushUncommittedChanges();

            // Save the uncommitted changes.
            _store.Save(aggregate, events);

            // The event repository is not responsible for publishing these events. Instead they are returned to the 
            // caller for that purpose.
            return events;
        }

        /// <summary>
        /// Loads an aggregate instance from the full history of events for that aggreate.
        /// </summary>
        private T Rehydrate<T>(Guid id) where T : AggregateRoot
        {
            // Get all the events for the aggregate.
            var events = _store.Get(id, -1);

            // Disallow empty event streams.
            if (!events.Any())
                throw new AggregateNotFoundException(typeof(T), id);

            // Create and load the aggregate.
            var aggregate = AggregateFactory<T>.CreateAggregate();
            aggregate.Rehydrate(events);
            return aggregate;
        }

        #region Methods (boxing and unboxing)

        /// <summary>
        /// Copies an aggregate to offline storage and removes it from online logs.
        /// </summary>
        /// <remarks>
        /// Aggregate boxing/unboxing is not implemented by default for all aggregates. It must be explicitly 
        /// implemented per aggregate for those aggregates that require this functionality, and snapshots are required. 
        /// Therefore this function in this class throws a NotImplementedException; refer to SnapshotRepository for the
        /// implementation.
        /// </remarks>
        public void Box<T>(T aggregate) where T : AggregateRoot
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an aggregate from offline storage and returns only its most recent state.
        /// </summary>
        /// <remarks>
        /// Aggregate boxing/unboxing is not implemented by default for all aggregates. It must be explicitly 
        /// implemented per aggregate for those aggregates that require this functionality, and snapshots are required. 
        /// Therefore this function in this class throws a NotImplementedException; refer to SnapshotRepository for the
        /// implementation.
        /// </remarks>
        public T Unbox<T>(Guid aggregateId) where T : AggregateRoot
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
