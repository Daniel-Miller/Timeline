using Sample.Domain;

using Timeline.Events;

namespace Sample.Application.Read
{
    public class TransferEventSubscriber
    {
        private readonly IQueryStore _store;
        private readonly IQuerySearch _search;

        public TransferEventSubscriber(IEventQueue queue, IQueryStore store, IQuerySearch search)
        {
            queue.Subscribe<TransferStarted>(Handle);
            queue.Subscribe<TransferUpdated>(Handle);
            queue.Subscribe<TransferCompleted>(Handle);

            _store = store;
            _search = search;
        }

        public void Handle(TransferStarted c)
        {
            _store.InsertTransfer(c.IdentityTenant, c.AggregateIdentifier, c.Status.ToString(), c.FromAccount, c.ToAccount, c.Amount);
        }

        public void Handle(TransferUpdated c)
        {
            _store.UpdateTransfer(c.AggregateIdentifier, c.Status.ToString(), c.Activity);
        }

        public void Handle(TransferCompleted c)
        {
            _store.UpdateTransfer(c.AggregateIdentifier, c.Status.ToString(), null);
        }
    }
}