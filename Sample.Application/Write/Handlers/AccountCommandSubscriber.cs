using Sample.Domain;

using Timeline.Commands;
using Timeline.Events;

namespace Sample.Application.Write
{
	public class AccountCommandSubscriber
	{
		private readonly IEventRepository _repository;
		private readonly IEventQueue _publisher;

		public AccountCommandSubscriber(ICommandQueue commander, IEventQueue publisher, IEventRepository repository)
		{
			_repository = repository;
			_publisher = publisher;

			commander.Subscribe<OpenAccount>(Handle);
			commander.Subscribe<DepositMoney>(Handle);
			commander.Subscribe<WithdrawMoney>(Handle);
			commander.Subscribe<CloseAccount>(Handle);
		}

		private void Commit(AccountAggregate aggregate)
		{
			var changes = _repository.Save(aggregate);
			foreach (var change in changes)
				_publisher.Publish(change);
		}

		public void Handle(OpenAccount c)
		{
			var aggregate = new AccountAggregate { AggregateIdentifier = c.AggregateIdentifier };
			aggregate.OpenAccount(c.Owner, c.Code);
			Commit(aggregate);
		}

		public void Handle(DepositMoney c)
		{
			var aggregate = _repository.Get<AccountAggregate>(c.AggregateIdentifier);
			aggregate.DepositMoney(c.Amount, c.Transaction);
			Commit(aggregate);
		}

		public void Handle(WithdrawMoney c)
		{
			var aggregate = _repository.Get<AccountAggregate>(c.AggregateIdentifier);
			aggregate.WithdrawMoney(c.Amount, c.Transaction);
			Commit(aggregate);
		}

		public void Handle(CloseAccount c)
		{
			var aggregate = _repository.Get<AccountAggregate>(c.AggregateIdentifier);
			aggregate.CloseAccount(c.Reason);
			Commit(aggregate);
		}
	}
}
