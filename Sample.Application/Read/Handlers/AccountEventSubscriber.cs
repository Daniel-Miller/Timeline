using Sample.Domain;

using Timeline.Events;

namespace Sample.Application.Read
{
    public class AccountEventSubscriber
    {
        private readonly IQueryStore _store;
        private readonly IQuerySearch _search;

        public AccountEventSubscriber(IEventQueue queue, IQueryStore store, IQuerySearch search)
        {
            queue.Subscribe<AccountOpened>(Handle);
            queue.Subscribe<AccountClosed>(Handle);
            queue.Subscribe<MoneyDeposited>(Handle);
            queue.Subscribe<MoneyWithdrawn>(Handle);

            _store = store;
            _search = search;
        }

        public void Handle(AccountOpened c)
        {
            _store.InsertAccount(c.IdentityTenant, c.AggregateIdentifier, c.Code, "Open", c.Owner);
        }

        public void Handle(AccountClosed c)
        {
            var account = _search.SelectAccountSummary(c.AggregateIdentifier);
            _store.UpdateAccountStatus(account.AccountIdentifier, "Closed");
        }

        public void Handle(MoneyDeposited c)
        {
            _store.IncreaseAccountBalance(c.AggregateIdentifier, c.Amount);
        }

        public void Handle(MoneyWithdrawn c)
        {
            _store.DecreaseAccountBalance(c.AggregateIdentifier, c.Amount);
        }
    }
}