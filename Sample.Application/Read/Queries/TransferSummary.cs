using System;

namespace Sample.Application.Read
{
    public class TransferSummary
    {
        public Guid TenantIdentifier { get; set; }

        public Guid TransferIdentifier { get; set; }
        public String TransferStatus { get; set; }
        public string TransferActivity { get; set; }
        public Decimal TransferAmount { get; set; }

        public Guid FromAccountIdentifier { get; set; }
        public String FromAccountOwner { get; set; }

        public Guid ToAccountIdentifier { get; set; }
        public String ToAccountOwner { get; set; }
    }
}
