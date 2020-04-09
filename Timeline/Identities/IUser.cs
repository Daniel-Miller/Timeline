using System;

namespace Timeline.Identities
{
    /// <summary>
    /// Represents a user who may or may not be authenticated.
    /// </summary>
    public interface IUser
    {
        Guid Identifier { get; }
        string Email { get; }
        string Name { get; }
        bool IsAuthenticated { get; }
        int Key { get; }
    }
}