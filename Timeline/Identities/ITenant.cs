using System;

namespace Timeline.Identities
{
    /// <summary>
    /// Represents a tenant in a multitenant system.
    /// </summary>
    public interface ITenant
    {
        Guid Identifier { get; }
        string Code { get; }
        string Name { get; }
        int Key { get; }
    }
}
