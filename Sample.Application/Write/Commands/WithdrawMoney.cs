using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class WithdrawMoney : Command
    {
        public decimal Amount { get; set; }
        public Guid Transaction { get; set; }

        public WithdrawMoney(Guid account, decimal amount, Guid transaction)
        {
            AggregateIdentifier = account;
            Amount = amount;
            Transaction = transaction;
        }
    }
}
