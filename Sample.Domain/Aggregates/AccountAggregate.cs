using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class AccountAggregate : AggregateRoot<Account>
    {
        public void OpenAccount(Guid owner, string code)
        {
            var e = new AccountOpened(owner, code);
            Apply(e, State.When);
        }

        public void DepositMoney(decimal amount, Guid transaction)
        {
            var e = new MoneyDeposited(amount, transaction);
            Apply(e, State.When);
        }

        public void WithdrawMoney(decimal amount, Guid transaction)
        {
            var e = new MoneyWithdrawn(amount, transaction);
            Apply(e, State.When);
        }

        public void CloseAccount(string reason)
        {
            var e = new AccountClosed(reason);
            Apply(e, State.When);
        }
    }
}
