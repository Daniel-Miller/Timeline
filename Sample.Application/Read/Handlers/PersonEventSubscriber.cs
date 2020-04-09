using Sample.Domain;

using Timeline.Events;

namespace Sample.Application.Read
{
    public class PersonEventSubscriber
    {
        private readonly IQueryStore _store;

        public PersonEventSubscriber(IEventQueue queue, IQueryStore store)
        {
            _store = store;

            queue.Subscribe<PersonBoxed>(Handle);
            queue.Subscribe<PersonRegistered>(Handle);
            queue.Subscribe<PersonRenamed>(Handle);
            queue.Subscribe<PersonUnboxed>(Handle);
        }

        public void Handle(PersonBoxed c)
        {
            _store.DeletePerson(c.AggregateIdentifier);
        }

        public void Handle(PersonRegistered c)
        {
            _store.InsertPerson(c.IdentityTenant, c.AggregateIdentifier, c.FirstName + " " + c.LastName, c.Registered);
        }

        public void Handle(PersonRenamed c)
        {
            _store.UpdatePersonName(c.AggregateIdentifier, c.FirstName + " " + c.LastName);
        }

        public void Handle(PersonUnboxed c)
        {
            _store.InsertPerson(c.IdentityTenant, c.AggregateIdentifier, c.Person.FirstName + " " + c.Person.LastName, c.Person.Registered);
        }
    }
}
