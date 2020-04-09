using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class StartTransfer : Command
    {
        public Guid FromAccount { get; set; }
        public Guid ToAccount { get; set; }
        public decimal Amount { get; set; }

        public StartTransfer(Guid id, Guid fromAccount, Guid toAccount, decimal amount)
        {
            AggregateIdentifier = id;
            FromAccount = fromAccount;
            ToAccount = toAccount;
            Amount = amount;
        }
    }
}
