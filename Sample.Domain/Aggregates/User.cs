using Timeline.Events;

namespace Sample.Domain
{
    public class User : AggregateState
    {
        public string LoginName { get; set; }
        public string LoginPassword { get; set; }
        public string Status { get; set; }

        public void When(UserRegistrationStarted @event)
        {
            LoginName = @event.Name;
            LoginPassword = @event.Password;
            Status = "Started";
        }

        public void When(UserRegistrationSucceeded @event)
        {
            Status = "Succeeded";
        }

        public void When(UserRegistrationFailed @event)
        {
            Status = "Failed";
        }
    }
}
