using Sample.Application.Read;

using Timeline.Identities;

namespace Sample.Presentation.Console.Identities
{
    public class CurrentIdentity : IIdentity
    {
        private readonly Tenant _current = null;

        public CurrentIdentity(Tenant current)
        {
            _current = current;
        }

        public ITenant Tenant => new CurrentTenant(_current);

        public IUser User => new CurrentUser();
    }
}