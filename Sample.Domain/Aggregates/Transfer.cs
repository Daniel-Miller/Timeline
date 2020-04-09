using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class Transfer : AggregateState
    {
        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }
        public decimal Amount { get; set; }
        public TransferStatus Status { get; set; }

        public void When(TransferStarted e)
        {
            FromAccount = e.FromAccount;
            ToAccount = e.ToAccount;
            Amount = e.Amount;
            Status = TransferStatus.Started;
        }

        public void When(TransferUpdated e)
        {
            Status = TransferStatus.Updated;
        }

        public void When(TransferCompleted e)
        {
            Status = TransferStatus.Completed;
        }
    }
}
