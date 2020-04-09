using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class CompleteUserRegistration : Command
    {
        public string Status { get; set; }

        public CompleteUserRegistration(Guid id, string status)
        {
            AggregateIdentifier = id;
            Status = status;
        }
    }
}