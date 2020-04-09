using Timeline.Events;

namespace Sample.Domain
{
    public class TransferCompleted : Event
    {
        public TransferStatus Status => TransferStatus.Completed;

        public TransferCompleted()
        {

        }
    }
}
