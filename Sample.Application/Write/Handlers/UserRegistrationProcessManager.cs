using Sample.Application.Read;
using Sample.Domain;

using Timeline.Commands;
using Timeline.Events;

namespace Sample.Application.Write
{
	public class UserRegistrationProcessManager
	{
		private readonly ICommandQueue _commander;
		private readonly IQuerySearch _querySearch;

		public UserRegistrationProcessManager(ICommandQueue commander, IEventQueue publisher, IQuerySearch querySearch)
		{
			_commander = commander;
			_querySearch = querySearch;

			publisher.Subscribe<UserRegistrationStarted>(Handle);
			publisher.Subscribe<UserRegistrationSucceeded>(Handle);
			publisher.Subscribe<UserRegistrationFailed>(Handle);
		}

		public void Handle(UserRegistrationStarted e)
		{
			// Registration succeeds only if no other user has the same login name.
			var status = _querySearch
				.UserExists(u => u.LoginName == e.Name && u.UserIdentifier != e.AggregateIdentifier)
				? "Failed" : "Succeeded";

			_commander.Send(new CompleteUserRegistration(e.AggregateIdentifier, status));
		}

		public void Handle(UserRegistrationSucceeded e) { }

		public void Handle(UserRegistrationFailed e) { }
	}
}
