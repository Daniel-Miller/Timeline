namespace Timeline.Identities
{
    /// <summary>
    /// An identity has two parts in a multitenant implementation: the tenant and the user.
    /// </summary>
    public interface IIdentity
    {
        ITenant Tenant { get; }
        IUser User { get; }
    }
}
