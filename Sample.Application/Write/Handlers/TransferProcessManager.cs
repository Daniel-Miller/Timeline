using System;

using Sample.Domain;

using Timeline.Commands;
using Timeline.Events;

namespace Sample.Application.Write
{
	public class TransferProcessManager
	{
		private readonly ICommandQueue _commander;
		private readonly IEventRepository _repository;

		public TransferProcessManager(ICommandQueue commander, IEventQueue publisher, IEventRepository repository)
		{
			_commander = commander;
			_repository = repository;

			publisher.Subscribe<TransferStarted>(Handle);
			publisher.Subscribe<MoneyDeposited>(Handle);
			publisher.Subscribe<MoneyWithdrawn>(Handle);
		}

		public void Handle(TransferStarted e)
		{
			var withdrawal = new WithdrawMoney(e.FromAccount, e.Amount, e.AggregateIdentifier);
			_commander.Send(withdrawal);
		}

		public void Handle(MoneyWithdrawn e)
		{
			if (e.Transaction == Guid.Empty)
				return;

			var status = new UpdateTransfer(e.Transaction, "Debit Succeeded");
			_commander.Send(status);

			var transfer = (Transfer) _repository.Get<TransferAggregate>(e.Transaction).State;

			var deposit = new DepositMoney(transfer.ToAccount, e.Amount, e.Transaction);
			_commander.Send(deposit);
		}

		public void Handle(MoneyDeposited e)
		{
			if (e.Transaction == Guid.Empty)
				return;

			var status = new UpdateTransfer(e.Transaction, "Credit Succeeded");
			_commander.Send(status);

			var complete = new CompleteTransfer(e.Transaction);
			_commander.Send(complete);
		}
	}
}
