using System;

namespace Sample.Application.Read
{
    public class UserSummary
    {
        public String LoginName { get; set; }
        public String LoginPassword { get; set; }
        public Guid UserIdentifier { get; set; }
        public String UserRegistrationStatus { get; set; }
    }
}
