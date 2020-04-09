using System;
using System.Diagnostics;
using System.Threading;

using Sample.Application.Read;
using Sample.Application.Write;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    /// <summary>
    /// Scenario 4: How to create a new user with a unique login name
    /// </summary>
    public class Test04
    {
        private ICommandQueue _commander;
        private IQuerySearch _querySearch;

        public Test04(ICommandQueue commander, IQuerySearch querySearch)
        {
            _commander = commander;
            _querySearch = querySearch;
        }
        
        public void Run()
        {
            var login = "jack@example.com";
            var password = "Let_Me_In!";

            if (RegisterUser(Guid.NewGuid(), login, password)) // This will succeed.
                System.Console.WriteLine($"User registration for {login} succeeded");
            
            if (!RegisterUser(Guid.NewGuid(), login, password)) // This will fail due to a duplicate login.
                System.Console.WriteLine($"User registration for {login} failed");
        }

        private bool RegisterUser(Guid id, string login, string password)
        {
            bool isComplete(Guid user) { return _querySearch.IsUserRegistrationCompleted(user); }
            const int waitTime = 200; // ms
            const int maximumRetries = 15; // 15 retries (~3 seconds)

            _commander.Send(new StartUserRegistration(id, login, password));

            for (var retry = 0; retry < maximumRetries && !isComplete(id); retry++)
                Thread.Sleep(waitTime);

            if (isComplete(id))
            {
                var summary = _querySearch.SelectUserSummary(id);
                return summary?.UserRegistrationStatus == "Succeeded";
            }
            else
            {
                var error = $"Registration for {login} has not completed after {waitTime * maximumRetries} ms";
                throw new IncompleteUserRegistrationException(error);
            }
        }

        private bool RegisterUserNoWait(Guid id, string login, string password)
        {
            bool isComplete(Guid user) { return _querySearch.IsUserRegistrationCompleted(user); }

            _commander.Send(new StartUserRegistration(id, login, password));

            Debug.Assert(isComplete(id));

            return _querySearch.SelectUserSummary(id)
                .UserRegistrationStatus == "Succeeded";
        }
    }
}
