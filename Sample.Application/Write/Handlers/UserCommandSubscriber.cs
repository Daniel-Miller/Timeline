using System;

using Sample.Domain;

using Timeline.Commands;
using Timeline.Events;

namespace Sample.Application.Write
{
    public class UserCommandSubscriber
    {
        private readonly IEventRepository _repository;
        private readonly IEventQueue _publisher;

        public UserCommandSubscriber(ICommandQueue commander, IEventQueue publisher, IEventRepository repository)
        {
            _repository = repository;
            _publisher = publisher;

            commander.Subscribe<StartUserRegistration>(Handle);
            commander.Subscribe<CompleteUserRegistration>(Handle);
        }

        private void Commit(UserAggregate aggregate)
        {
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(StartUserRegistration c)
        {
            var aggregate = new UserAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.StartRegistration(c.Name, c.Password);
            Commit(aggregate);
        }

        public void Handle(CompleteUserRegistration c)
        {
            var aggregate = _repository.Get<UserAggregate>(c.AggregateIdentifier);
            aggregate.CompleteRegistration(c.Status);
            Commit(aggregate);
        }
    }
}
