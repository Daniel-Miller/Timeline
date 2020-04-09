using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class Person : AggregateState
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset Registered { get; set; }

        public void When(PersonRegistered @event)
        {
            FirstName = @event.FirstName;
            LastName = @event.LastName;
            Registered = @event.Registered;
        }

        public void When(PersonRenamed @event)
        {
            FirstName = @event.FirstName;
            LastName = @event.LastName;
        }

        #region Methods (boxing and unboxing)

        public void When(PersonBoxed _)
        {
            // Nothing special needed here.
        }

        public void When(PersonUnboxed @event)
        {
            FirstName = @event.Person.FirstName;
            LastName = @event.Person.LastName;
            Registered = @event.Person.Registered;
        }

        #endregion
    }
}
