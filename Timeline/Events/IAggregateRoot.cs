using System;
using System.Collections.Generic;
using System.Text;

namespace Timeline.Events
{
    public interface IAggregateRoot
    {
        Guid AggregateIdentifier { get; set; }

        int AggregateVersion { get; set; }

        AggregateState State { get; set; }

        IEvent[] FlushUncommittedChanges();

        IEvent[] GetUncommittedChanges();

        void Rehydrate(IEnumerable<IEvent> history);
    }
}
