using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class TransferAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new Transfer();

        public void StartTransfer(Guid fromAccount, Guid toAccount, decimal amount)
        {
            var e = new TransferStarted(fromAccount, toAccount, amount);
            Apply(e);
        }

        public void UpdateTransfer(string activity)
        {
            var e = new TransferUpdated(activity);
            Apply(e);
        }

        public void CompleteTransfer()
        {
            var e = new TransferCompleted();
            Apply(e);
        }
    }
}
