using System;
using Timeline.Events;

namespace Timeline.Snapshots
{
    /// <summary>
    /// Represents a memento for a specific version of a specific aggregate.
    /// </summary>
    public class Snapshot
    {
        public Guid AggregateIdentifier { get; set; }
        public int AggregateVersion { get; set; }
        public AggregateState AggregateState { get; set; }
    }
}
