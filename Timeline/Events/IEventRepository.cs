using System;

namespace Timeline.Events
{
    /// <summary>
    /// Provides functionality to get and save aggregates.
    /// </summary>
    public interface IEventRepository
    {
        /// <summary>
        /// Returns the aggregate identified by the specified id.
        /// </summary>
        T Get<T>(Guid id) where T : IAggregateRoot;

        /// <summary>
        /// Saves an aggregate.
        /// </summary>
        /// <returns>
        /// Returns the events that are now saved (and ready to be published).
        /// </returns>
        IEvent[] Save<T>(T aggregate, int? version = null) where T : IAggregateRoot;

        /// <summary>
        /// Copies an aggregate to offline storage and removes it from online logs.
        /// </summary>
        void Box<T>(T aggregate) where T : IAggregateRoot;

        /// <summary>
        /// Retrieves an aggregate from offline storage and returns only its most recent state.
        /// </summary>
        T Unbox<T>(Guid aggregate) where T : IAggregateRoot;
    }
}
