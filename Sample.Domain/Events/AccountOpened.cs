using System;

using Timeline.Events;

namespace Sample.Domain
{
    public class AccountOpened : Event
    {
        public Guid Owner { get; set; }
        public string Code { get; set; }

        public AccountOpened(Guid owner, string code)
        {
            Owner = owner;
            Code = code;
        }
    }
}
