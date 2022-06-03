using System;
using System.Collections.Generic;
using System.Linq;

using Timeline.Exceptions;

namespace Timeline.Events
{
    /// <summary>
    /// Implements the base class for all aggregate roots. An aggregate forms a tree or graph of object relations. The 
    /// aggregate root is the top-level container, which speaks for the whole and may delegates down to the rest. It is 
    /// important because it is the one that the rest of the world communicates with.
    /// </summary>
    public abstract class AggregateRoot<TState> : IAggregateRoot
        where TState : AggregateState, new ()
    {
        /// <summary>
        /// Changes to the state of the aggregate that are not yet committed to a persistent event store.
        /// </summary>
        private readonly List<IEvent> _changes = new List<IEvent>();

        public AggregateRoot()
        {
            State = new TState();
        }

        /// <summary>
        /// Represents the state (i.e. data/packet) for the aggregate.
        /// </summary>
        public TState State { get; set; }

        AggregateState IAggregateRoot.State
        {
            get
            {
                return State;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                State = (TState)value;
            }
        }

        /// <summary>
        /// Uniquely identifies the aggregate.
        /// </summary>
        public Guid AggregateIdentifier { get; set; }

        /// <summary>
        /// Current version of the aggregate.
        /// </summary>
        public int AggregateVersion { get; set; }

        /// <summary>
        /// Returns all uncommitted changes. 
        /// </summary>
        /// <returns></returns>
        public IEvent[] GetUncommittedChanges()
        {
            lock (_changes)
            {
                return _changes.ToArray();
            }
        }

        /// <summary>
        /// Returns all uncommitted changes and clears them from the aggregate.
        /// </summary>
        public IEvent[] FlushUncommittedChanges()
        {
            lock (_changes)
            {
                var changes = _changes.ToArray();

                var i = 0;

                foreach (var change in changes)
                {
                    if (change.AggregateIdentifier == Guid.Empty && AggregateIdentifier == Guid.Empty)
                        throw new MissingAggregateIdentifierException(GetType(), change.GetType());

                    if (change.AggregateIdentifier == Guid.Empty)
                        change.AggregateIdentifier = AggregateIdentifier;

                    i++;

                    change.AggregateVersion = AggregateVersion + i;
                    change.EventTime = DateTimeOffset.UtcNow;
                }

                AggregateVersion = AggregateVersion + changes.Length;

                _changes.Clear();

                return changes;
            }
        }

        /// <summary>
        /// Loads an aggregate from a stream of events.
        /// </summary>
        public void Rehydrate(IEnumerable<IEvent> history)
        {
            lock (_changes)
            {
                foreach (var change in history.ToArray())
                {
                    if (change.AggregateVersion != AggregateVersion + 1)
                        throw new UnorderedEventsException(change.AggregateIdentifier);

                    ApplyEvent(change);

                    AggregateIdentifier = change.AggregateIdentifier;
                    AggregateVersion++;
                }
            }
        }

        /// <summary>
        /// Applies a change to the aggregate state AND appends the event to the history of uncommited changes.
        /// </summary>
        protected void Apply<T>(T changeEvent, Action<T> applyAction) where T : IEvent
        {
            lock (_changes)
            {
                applyAction(changeEvent);
                _changes.Add(changeEvent);
            }
        }

        /// <summary>
        /// Applies a change to the aggregate state. This method is called internally when rehydrating an aggregate, 
        /// and you can override this when custom handling is needed.
        /// </summary>
        protected virtual void ApplyEvent(IEvent change)
        {
            State.Apply(change);
        }
    }
}
