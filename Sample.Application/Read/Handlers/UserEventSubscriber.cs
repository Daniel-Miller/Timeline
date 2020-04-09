using Sample.Domain;

using Timeline.Events;

namespace Sample.Application.Read
{
    public class UserEventSubscriber
    {
        private readonly IQueryStore _store;

        public UserEventSubscriber(IEventQueue queue, IQueryStore store)
        {
            queue.Subscribe<UserRegistrationStarted>(Handle);
            queue.Subscribe<UserRegistrationSucceeded>(Handle);
            queue.Subscribe<UserRegistrationFailed>(Handle);

            _store = store;
        }

        public void Handle(UserRegistrationStarted c)
        {
            _store.InsertUser(c.AggregateIdentifier, c.Name, c.Password, "Started");
        }

        public void Handle(UserRegistrationSucceeded c)
        {
            _store.UpdateUserStatus(c.AggregateIdentifier, "Succeeded");
        }

        public void Handle(UserRegistrationFailed c)
        {
            _store.UpdateUserStatus(c.AggregateIdentifier, "Failed");
        }
    }
}
