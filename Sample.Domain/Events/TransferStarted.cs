using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class TransferStarted : Event
    {
        public TransferStatus Status => TransferStatus.Started;
        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }
        public decimal Amount { get; set; }

        public TransferStarted(Guid fromAccount, Guid toAccount, decimal amount)
        {
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
        }
    }
}
