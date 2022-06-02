using System;

namespace Timeline.Snapshots
{
    /// <summary>
    /// Defines the methods needed from the snapshot store.
    /// </summary>
    public interface ISnapshotStore
    {
        /// <summary>
        /// Gets a snapshot from the store.
        /// </summary>
        Snapshot Get(Guid id, Type aggregateRootType);

        /// <summary>
        /// Saves a snapshot to the store.
        /// </summary>
        void Save(Snapshot snapshot);

        /// <summary>
        /// Copies a snapshot to offline storage and removes it from online logs.
        /// </summary>
        void Box(Guid id);

        /// <summary>
        /// Retrieves an aggregate from offline storage and returns only its most recent state.
        /// </summary>
        Snapshot Unbox(Guid id, Type aggregateRootType);
    }
}
