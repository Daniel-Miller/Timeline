using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class RenamePerson : Command
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public RenamePerson(Guid id, string firstName, string lastName)
        {
            AggregateIdentifier = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
