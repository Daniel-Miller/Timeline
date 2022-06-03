using Timeline.Events;

namespace Sample.Domain
{
    public class UserAggregate : AggregateRoot<User>
    {
        public void StartRegistration(string name, string password)
        {
            var e = new UserRegistrationStarted(name, password);
            Apply(e, State.When);
        }

        public void CompleteRegistration(string status)
        {
            if (status == "Succeeded")
                Apply(new UserRegistrationSucceeded(), State.When);
            
            else if (status == "Failed")
                Apply(new UserRegistrationFailed(), State.When);
        }
    }
}
