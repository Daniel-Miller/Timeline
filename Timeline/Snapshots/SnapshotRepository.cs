using System;
using System.Linq;

using Timeline.Events;
using Timeline.Utilities;

namespace Timeline.Snapshots
{
    /// <summary>
    /// Saves and gets snapshots to and from a snapshot store.
    /// </summary>
    public class SnapshotRepository : IEventRepository
    {
        private readonly GuidCache<AggregateRoot> _cache = new GuidCache<AggregateRoot>();

        private readonly ISnapshotStore _snapshotStore;
        private readonly ISnapshotStrategy _snapshotStrategy;
        private readonly IEventRepository _eventRepository;
        private readonly IEventStore _eventStore;

        /// <summary>
        /// Constructs a new SnapshotRepository instance.
        /// </summary>
        /// <param name="eventStore">Store where events are persisted</param>
        /// <param name="eventRepository">Repository to get aggregates from the event store</param>
        /// <param name="snapshotStore">Store where snapshots are persisted</param>
        /// <param name="snapshotStrategy">Strategy used to determine when to take a snapshot</param>
        public SnapshotRepository(IEventStore eventStore, IEventRepository eventRepository, ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _eventRepository = eventRepository ?? throw new ArgumentNullException(nameof(eventRepository));
            _snapshotStore = snapshotStore ?? throw new ArgumentNullException(nameof(snapshotStore));
            _snapshotStrategy = snapshotStrategy ?? throw new ArgumentNullException(nameof(snapshotStrategy));
        }

        /// <summary>
        /// Saves the aggregate. Takes a snapshot if needed.
        /// </summary>
        public IEvent[] Save<T>(T aggregate, int? version = null) where T : AggregateRoot
        {
            // Cache the aggregate for 5 minutes.
            _cache.Add(aggregate.AggregateIdentifier, aggregate, 5 * 60, true);

            // Take a snapshot if needed.
            TakeSnapshot(aggregate, false);

            // Return the stream of saved events to the caller so they can be published.
            return _eventRepository.Save(aggregate, version);
        }

        /// <summary>
        /// Gets the aggregate.
        /// </summary>
        public T Get<T>(Guid aggregateId) where T : AggregateRoot
        {
            // Check the cache to see if the aggregate is already in memory.
            var snapshot = _cache.Get(aggregateId);
            if (snapshot != null)
                return (T)snapshot;

            // If it is not in the cache then load the aggregate from the most recent snapshot.
            var aggregate = AggregateFactory<T>.CreateAggregate();
            var snapshotVersion = RestoreAggregateFromSnapshot(aggregateId, aggregate);

            // If there is no snapshot then load the aggregate directly from the event store.
            if (snapshotVersion == -1)
                return _eventRepository.Get<T>(aggregateId);

            // Otherwise load the aggregate from the events that occurred after the snapshot was taken.
            var events = (_eventStore.Get(aggregateId, snapshotVersion))
                .Where(desc => desc.AggregateVersion > snapshotVersion);

            aggregate.Rehydrate(events);

            return aggregate;
        }

        /// <summary>
        /// Loads the aggregate from the most recent snapshot.
        /// </summary>
        /// <returns>
        /// Returns the version number for the aggregate when the snapshot was taken.
        /// </returns>
        private int RestoreAggregateFromSnapshot<T>(Guid id, T aggregate) where T : AggregateRoot
        {
            var snapshot = _snapshotStore.Get(id, typeof(T));

            if (snapshot == null)
                return -1;

            // TODO: Is this needed when only the version is returned?

            // aggregate.AggregateIdentifier = snapshot.AggregateIdentifier;
            // aggregate.AggregateVersion = snapshot.AggregateVersion;
            // aggregate.State = _eventStore.Serializer.Deserialize<AggregateState>(snapshot.AggregateState, aggregate.CreateState().GetType());

            return snapshot.AggregateVersion;
        }

        /// <summary>
        /// Saves a snapshot of the aggregate if the strategy indicates a snapshot should now be taken.
        /// </summary>
        private void TakeSnapshot(AggregateRoot aggregate, bool force)
        {
            if (!force && !_snapshotStrategy.ShouldTakeSnapShot(aggregate))
                return;

            var snapshot = new Snapshot
            {
                AggregateIdentifier = aggregate.AggregateIdentifier,
                AggregateVersion = aggregate.AggregateVersion,
                AggregateState = aggregate.State
            };

            snapshot.AggregateVersion = aggregate.AggregateVersion + aggregate.GetUncommittedChanges().Length;

            _snapshotStore.Save(snapshot);
        }

        #region Methods (boxing and unboxing)

        /// <summary>
        /// Checks for expired aggregates. Automatically boxes all aggregates for which the timer is now elapsed.
        /// </summary>
        public void Ping()
        {
            var aggregates = _eventStore.GetExpired(DateTimeOffset.UtcNow);
            foreach (var aggregate in aggregates)
                Box(Get<AggregateRoot>(aggregate));
        }

        /// <summary>
        /// Copies an aggregate to offline storage and removes it from online logs.
        /// </summary>
        public void Box<T>(T aggregate) where T : AggregateRoot
        {
            TakeSnapshot(aggregate, true);

            _snapshotStore.Box(aggregate.AggregateIdentifier);
            _eventStore.Box(aggregate.AggregateIdentifier);

            _cache.Remove(aggregate.AggregateIdentifier);
        }

        /// <summary>
        /// Retrieves an aggregate from offline storage and returns only its most recent state.
        /// </summary>
        public T Unbox<T>(Guid aggregateId) where T : AggregateRoot
        {
            var snapshot = _snapshotStore.Unbox(aggregateId, typeof(T));
            var aggregate = AggregateFactory<T>.CreateAggregate();
            aggregate.AggregateIdentifier = aggregateId;
            aggregate.AggregateVersion = 1;
            aggregate.State = snapshot.AggregateState;
            return aggregate;
        }

        #endregion
    }
}
