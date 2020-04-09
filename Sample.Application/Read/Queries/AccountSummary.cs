using System;

namespace Sample.Application.Read
{
    public class AccountSummary
    {
        public Guid TenantIdentifier { get; set; }

        public Guid OwnerIdentifier { get; set; }
        public String OwnerName { get; set; }

        public String AccountCode { get; set; }
        public Guid AccountIdentifier { get; set; }
        public String AccountStatus { get; set; }
        public Decimal AccountBalance { get; set; }

    }
}
