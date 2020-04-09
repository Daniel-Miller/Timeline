using System;

using Sample.Domain;

using Timeline.Commands;
using Timeline.Events;

namespace Sample.Application.Write
{
    public class PersonCommandSubscriber
    {
        private readonly IEventRepository _repository;
        private readonly IEventQueue _publisher;

        public PersonCommandSubscriber(ICommandQueue commander, IEventQueue publisher, IEventRepository repository)
        {
            _repository = repository;
            _publisher = publisher;

            commander.Subscribe<RegisterPerson>(Handle);
            commander.Subscribe<RenamePerson>(Handle);

            commander.Subscribe<BoxPerson>(Handle);
            commander.Subscribe<UnboxPerson>(Handle);
        }

        private void Commit(PersonAggregate aggregate)
        {
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(RegisterPerson c)
        {
            var aggregate = new PersonAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.RegisterPerson(c.FirstName, c.LastName, DateTimeOffset.UtcNow);
            Commit(aggregate);
        }

        public void Handle(RenamePerson c)
        {
            var aggregate = _repository.Get<PersonAggregate>(c.AggregateIdentifier);
            aggregate.RenamePerson(c.FirstName, c.LastName);
            Commit(aggregate);
        }

        public void Handle(BoxPerson c)
        {
            var aggregate = _repository.Get<PersonAggregate>(c.AggregateIdentifier);
            aggregate.BoxPerson();
            Commit(aggregate);

            _repository.Box(aggregate);
        }

        public void Handle(UnboxPerson c)
        {
            var aggregate = _repository.Unbox<PersonAggregate>(c.AggregateIdentifier);
            aggregate.UnboxPerson((Person)aggregate.State);
            Commit(aggregate);
        }
    }
}
