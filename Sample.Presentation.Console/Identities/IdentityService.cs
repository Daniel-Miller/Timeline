
using Timeline.Identities;

namespace Sample.Presentation.Console.Identities
{
    public class IdentityService : IIdentityService
    {
        public IIdentity GetCurrent()
        {
            return new CurrentIdentity(ProgramSettings.CurrentTenant);
        }
    }
}