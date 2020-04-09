using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class DepositMoney : Command
    {
        public decimal Amount { get; set; }
        public Guid Transaction { get; set; }

        public DepositMoney(Guid account, decimal amount, Guid transaction)
        {
            AggregateIdentifier = account;
            Amount = amount;
            Transaction = transaction;
        }

        public DepositMoney(Guid account, decimal amount)
            : this(account,amount,Guid.Empty)
        {
            
        }
    }
}
