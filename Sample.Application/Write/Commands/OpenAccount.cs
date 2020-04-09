using System;

using Timeline.Commands;

namespace Sample.Application.Write
{
    public class OpenAccount : Command
    {
        public Guid Owner { get; set; }
        public string Code { get; set; }

        public OpenAccount(Guid id, Guid owner, string code)
        {
            AggregateIdentifier = id;
            Owner = owner;
            Code = code;
        }
    }
}
