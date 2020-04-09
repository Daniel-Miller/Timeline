using System;

using Timeline.Identities;

namespace Sample.Presentation.Console.Identities
{
    public class CurrentUser : IUser
    {
        public Guid Identifier => Guid.Empty;
        public string Name => "root";
        public string Email => "root@example.com";
        public bool IsAuthenticated => true;
        public int Key => 0;
    }
}