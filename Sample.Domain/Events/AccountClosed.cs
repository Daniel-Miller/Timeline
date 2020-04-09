using Timeline.Events;

namespace Sample.Domain
{
    public class AccountClosed : Event
    {
        public string Reason { get; set; }

        public AccountClosed(string reason)
        {
            Reason = reason;
        }
    }
}
