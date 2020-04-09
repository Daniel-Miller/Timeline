using Sample.Domain;

using Timeline.Commands;
using Timeline.Events;

namespace Sample.Application.Write
{
    public class TransferCommandSubscriber
    {
        private readonly IEventRepository _repository;
        private readonly IEventQueue _publisher;

        public TransferCommandSubscriber(ICommandQueue commander, IEventQueue publisher, IEventRepository repository)
        {
            _repository = repository;
            _publisher = publisher;

            commander.Subscribe<StartTransfer>(Handle);
            commander.Subscribe<UpdateTransfer>(Handle);
            commander.Subscribe<CompleteTransfer>(Handle);
        }

        private void Commit(TransferAggregate aggregate)
        {
            var changes = _repository.Save(aggregate);
            foreach (var change in changes)
                _publisher.Publish(change);
        }

        public void Handle(StartTransfer c)
        {
            var aggregate = new TransferAggregate { AggregateIdentifier = c.AggregateIdentifier };
            aggregate.StartTransfer(c.FromAccount, c.ToAccount, c.Amount);
            Commit(aggregate);
        }

        public void Handle(UpdateTransfer c)
        {
            var aggregate = _repository.Get<TransferAggregate>(c.AggregateIdentifier);
            aggregate.UpdateTransfer(c.Activity);
            Commit(aggregate);
        }

        public void Handle(CompleteTransfer c)
        {
            var aggregate = _repository.Get<TransferAggregate>(c.AggregateIdentifier);
            aggregate.CompleteTransfer();
            Commit(aggregate);
        }
    }
}
