using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class UnboxPerson : Command
    {
        public UnboxPerson(Guid id)
        {
            AggregateIdentifier = id;
        }
    }
}
