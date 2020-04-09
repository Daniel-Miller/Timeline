using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class MoneyDeposited : Event
    {
        public decimal Amount { get; set; }
        public Guid Transaction { get; set; }
        
        public MoneyDeposited(decimal amount, Guid transaction)
        {
            Amount = amount;
            Transaction = transaction;
        }
    }
}
