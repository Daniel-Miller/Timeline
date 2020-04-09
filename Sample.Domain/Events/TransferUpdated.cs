using Timeline.Events;

namespace Sample.Domain
{
    public class TransferUpdated : Event
    {
        public TransferStatus Status => TransferStatus.Updated;
        public string Activity { get; set; }

        public TransferUpdated(string activity)
        {
            Activity = activity;
        }
    }
}
