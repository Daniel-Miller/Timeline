using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class Account : AggregateState
    {
        public Guid Owner { get; set; }
        public decimal CurrentBalance { get; set; }
        public string CurrentStatus { get; set; }
        public bool IsOverdrawn => CurrentBalance < 0;

        public void When(MoneyDeposited e)
        {
            CurrentBalance += e.Amount;
        }

        public void When(MoneyWithdrawn e)
        {
            CurrentBalance -= e.Amount;
        }

        public void When(AccountOpened e)
        {
            Owner = e.Owner;
            CurrentStatus = "Open";
        }

        public void When(AccountClosed e)
        {
            CurrentStatus = "Closed";
        }
    }
}
