using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class PersonAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new Person();

        public void BoxPerson()
        {
            var e = new PersonBoxed();
            Apply(e);
        }

        public void UnboxPerson(Person person)
        {
            var e = new PersonUnboxed(person);
            Apply(e);
        }

        public void RegisterPerson(string firstName, string lastName, DateTimeOffset registered)
        {
            var e = new PersonRegistered(firstName, lastName, registered);
            Apply(e);
        }

        public void RenamePerson(string firstName, string lastName)
        {
            var e = new PersonRenamed(firstName, lastName);
            Apply(e);
        }
    }
}
