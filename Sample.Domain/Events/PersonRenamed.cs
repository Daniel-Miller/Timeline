using Timeline.Events;

namespace Sample.Domain
{
    public class PersonRenamed : Event
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public PersonRenamed(string first, string last) { FirstName = first; LastName = last; }
    }
}
