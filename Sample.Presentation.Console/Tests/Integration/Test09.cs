using System;
using System.Threading;
using System.Threading.Tasks;

using Sample.Application.Write;
using Sample.Presentation.Console.Resources;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    public static class Test09
    {
        /// <summary>
        /// Implements a basic stress test. Sends the specified number of commands to the specified number of tasks in
        /// parallel.
        /// </summary>
        public static void Run(ICommandQueue commander, int taskCount, int commandCount, int maxSleep)
        {
            var gen = new DataGenerator();

            Parallel.For(0, taskCount, (i) =>
            {
                var firstName = gen.RandomFirstName();

                // Register a new person with a random name.
                var person = Guid.NewGuid();
                commander.Send(new RegisterPerson(person, firstName, gen.RandomLastName()));

                // Open a new bank account.
                var account = Guid.NewGuid();
                commander.Send(new OpenAccount(account, person, $"Account {gen.Sequence()}"));

                // Deposit some money.
                commander.Send(new DepositMoney(account, gen.RandomInteger(1, 100), Guid.Empty));
                commander.Send(new DepositMoney(account, gen.RandomInteger(1, 100), Guid.Empty));

                // Change the person's name multiple names.
                for (var j = 0; j < commandCount; j++)
                {
                    commander.Send(new RenamePerson(person, firstName, gen.RandomLastName()));

                    // Sleep for some random length of time.
                    if (maxSleep > 0)
                        Thread.Sleep(gen.RandomInteger(0, maxSleep));
                }

                // Schedule a future name change.
                commander.Schedule(new RenamePerson(person, firstName, gen.RandomLastName()), DateTimeOffset.Now);

                // Withdraw some money.
                commander.Send(new WithdrawMoney(account, gen.RandomInteger(1, 100), Guid.Empty));

                // Close bank account.
                commander.Send(new CloseAccount(account, "None"));
            });

            // Trigger scheduled commands.
            commander.Ping();
        }
    }
}
