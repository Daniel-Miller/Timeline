using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class RegisterPerson : Command
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public RegisterPerson(Guid id, string firstName, string lastName)
        {
            AggregateIdentifier = id;
            FirstName = firstName;
            LastName = lastName;
        }
    }
}