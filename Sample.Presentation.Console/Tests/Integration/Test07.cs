using System;

using Sample.Application.Read;
using Sample.Application.Write;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    /// <summary>
    /// Scenario G: How to implement a custom event handler
    /// </summary>
    public class Test07
    {
        private ICommandQueue _commander;

        public Test07(ICommandQueue commander)
        {
            _commander = commander;

            ProgramSettings.CurrentTenant = Tenants.Umbrella;
        }

        public void Run()
        {
            // Start one account with $50,000.
            var ada = Guid.NewGuid();
            CreatePerson(ada, "Ada", "Wong");
            var a = Guid.NewGuid();
            StartAccount(ada, a, "Ada's Account", 50000);

            // Start another account with $25,000.
            var albert = Guid.NewGuid();
            CreatePerson(albert, "Albert", "Wesker");
            var b = Guid.NewGuid();
            StartAccount(albert, b, "Albert's Account", 100);

            // Create a money transfer for Ada giving money to Albert.
            var tx = Guid.NewGuid();
            _commander.Send(new StartTransfer(tx, a, b, 18000));
        }

        private void StartAccount(Guid person, Guid account, string code, decimal deposit)
        {
            _commander.Send(new OpenAccount(account, person, code));
            _commander.Send(new DepositMoney(account, deposit));
        }

        private void CreatePerson(Guid person, string first, string last)
        {
            _commander.Send(new RegisterPerson(person, first, last));
        }
    }
}
