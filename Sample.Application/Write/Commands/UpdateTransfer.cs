using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class UpdateTransfer : Command
    {
        public string Activity { get; set; }

        public UpdateTransfer(Guid id, string activity)
        {
            AggregateIdentifier = id;
            Activity = activity;
        }
    }
}
