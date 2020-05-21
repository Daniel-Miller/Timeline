using System;

namespace Timeline.Commands
{
    /// <summary>
    /// In a multi-tenant system we need to allow an individual tenant to override/customize the handling of a 
    /// command. In this case the class name and the tenant identifier are used together as the unique key.
    /// </summary>
    public class CommandOverrideKey
    {
        public string CommandName { get; set; }

        public Guid IdentityTenant { get; set; }
    }
}
