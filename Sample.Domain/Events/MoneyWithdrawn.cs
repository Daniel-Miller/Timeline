using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class MoneyWithdrawn : Event
    {
        public decimal Amount { get; set; }
        public Guid Transaction { get; set; }
        
        public MoneyWithdrawn(decimal amount, Guid transaction)
        {
            Amount = amount;
            Transaction = transaction;
        }
    }
}
