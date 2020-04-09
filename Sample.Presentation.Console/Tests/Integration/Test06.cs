using System;

using Sample.Application.Write;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    /// <summary>
    /// Scenario F: How to update multiple aggregates with one command
    /// </summary>
    public class Test06
    {
        private ICommandQueue _commander;

        public Test06(ICommandQueue commander)
        {
            _commander = commander;
        }

        public void Run()
        {
            // Start one account with $100.
            var bill = Guid.NewGuid();
            CreatePerson(bill, "Bill", "Esquire");
            var blue = Guid.NewGuid();
            StartAccount(bill, blue, "Bill's Blue Account", 100);

            // Start another account with $100.
            var ted = Guid.NewGuid();
            CreatePerson(ted, "Ted", "Logan");
            var red = Guid.NewGuid();
            StartAccount(ted, red, "Ted's Red Account", 100);

            // Create a money transfer for Bill giving money to Ted.
            var tx = Guid.NewGuid();
            _commander.Send(new StartTransfer(tx, blue, red, 69));
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
