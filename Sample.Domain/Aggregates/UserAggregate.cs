using Timeline.Events;

namespace Sample.Domain
{
    public class UserAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new User();

        public void StartRegistration(string name, string password)
        {
            var e = new UserRegistrationStarted(name, password);
            Apply(e);
        }

        public void CompleteRegistration(string status)
        {
            if (status == "Succeeded")
                Apply(new UserRegistrationSucceeded());
            
            else if (status == "Failed")
                Apply(new UserRegistrationFailed());
        }
    }
}
