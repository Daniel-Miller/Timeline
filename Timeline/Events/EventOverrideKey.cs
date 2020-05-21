using System;

namespace Timeline.Events
{
    /// <summary>
    /// In a multi-tenant system we may want to allow each individual tenant to override/customize the handling of 
    /// an event. The class name and the tenant identifier is used as the unique key here.
    /// </summary>
    internal class EventOverrideKey
    {
        public string EventName { get; set; }

        public Guid IdentityTenant { get; set; }
    }
}
