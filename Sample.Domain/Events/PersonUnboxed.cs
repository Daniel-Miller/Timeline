using Timeline.Events;

namespace Sample.Domain
{
    public class PersonUnboxed : Event
    {
        public Person Person { get; set; }

        public PersonUnboxed(Person person)
        {
            Person = person;
        }
    }
}
