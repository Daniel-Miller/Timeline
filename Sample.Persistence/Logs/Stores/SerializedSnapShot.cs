using System;
using System.Collections.Generic;
using System.Text;

namespace Sample.Persistence.Logs.Stores
{
    public class SerializedSnapShot
    {
        public Guid AggregateIdentifier { get; set; }
        public int AggregateVersion { get; set; }
        public string AggregateState { get; set; }
    }
}
