using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class PersonAggregate : AggregateRoot<Person>
    {
        public void BoxPerson()
        {
            var e = new PersonBoxed();
            Apply(e, State.When);
        }

        public void UnboxPerson(Person person)
        {
            var e = new PersonUnboxed(person);
            Apply(e, State.When);
        }

        public void RegisterPerson(string firstName, string lastName, DateTimeOffset registered)
        {
            var e = new PersonRegistered(firstName, lastName, registered);
            Apply(e, State.When);
        }

        public void RenamePerson(string firstName, string lastName)
        {
            var e = new PersonRenamed(firstName, lastName);
            Apply(e, State.When);
        }
    }
}
