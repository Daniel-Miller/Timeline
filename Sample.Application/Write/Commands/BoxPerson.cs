using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class BoxPerson : Command
    {
        public BoxPerson(Guid id)
        {
            AggregateIdentifier = id;
        }
    }
}
