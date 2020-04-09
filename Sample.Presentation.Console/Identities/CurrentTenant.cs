using System;

using Sample.Application.Read;

using Timeline.Identities;

namespace Sample.Presentation.Console.Identities
{
    public class CurrentTenant : ITenant
    {
        private readonly Tenant _tenant;

        public CurrentTenant(Tenant tenant)
        {
            _tenant = tenant;
        }

        public Guid Identifier => _tenant.Identifier;
        public string Code => _tenant.Code;
        public string Name => _tenant.Name;
        public int Key => _tenant.Key;
    }
}