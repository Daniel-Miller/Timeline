using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class StartUserRegistration : Command
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public StartUserRegistration(Guid id, string name, string password)
        {
            AggregateIdentifier = id;
            Name = name;
            Password = password;
        }
    }
}