using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class CloseAccount : Command
    {
        public string Reason { get; set; }

        public CloseAccount(Guid account, string reason)
        {
            AggregateIdentifier = account;
            Reason = reason;
        }
    }
}
