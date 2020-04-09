namespace Timeline.Identities
{
    /// <summary>
    /// Provides a method to determine the identity of the current tenant and user. This establishes the bare-metal 
    /// context for the session in which commands are sent and events are published.
    /// </summary>
    public interface IIdentityService
    {
        IIdentity GetCurrent();
    }
}
