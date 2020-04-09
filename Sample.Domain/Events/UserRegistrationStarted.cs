using Timeline.Events;

namespace Sample.Domain
{
    public class UserRegistrationStarted : Event
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public UserRegistrationStarted(string name, string password)
        {
            Name = name;
            Password = password;
        }
    }
}
