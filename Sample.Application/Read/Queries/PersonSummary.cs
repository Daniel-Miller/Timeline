using System;

namespace Sample.Application.Read
{
    public class PersonSummary
    {
        public Guid TenantIdentifier { get; set; }

        public Guid PersonIdentifier { get; set; }
        public string PersonName { get; set; }
        public DateTimeOffset PersonRegistered { get; set; }

        public int OpenAccountCount { get; set; }
        public decimal TotalAccountBalance { get; set; }
    }
}
