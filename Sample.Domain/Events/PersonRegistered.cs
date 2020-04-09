using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class PersonRegistered : Event
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset Registered { get; set; }

        public PersonRegistered(string first, string last, DateTimeOffset registered)
        {
            FirstName = first; 
            LastName = last; 
            Registered = registered;
        }
    }
}
