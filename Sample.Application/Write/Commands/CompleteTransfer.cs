using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class CompleteTransfer : Command
    {
        public CompleteTransfer(Guid id)
        {
            AggregateIdentifier = id;
        }
    }
}
