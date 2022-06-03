using System;
using System.Threading;
using Sample.Application.Write;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    /// <summary>
    /// Scenario E: How to schedule a command
    /// </summary>
    public static class Test05
    {
        public static void Run(ICommandQueue commander)
        {
            var alice = Guid.NewGuid();
            var tomorrow = DateTimeOffset.UtcNow.AddDays(-1);
            commander.Schedule(new RegisterPerson(alice, "Alice", "O'Wonderland"), tomorrow);

            // After the above timer elapses, any call to Ping() executes the scheduled command.
            Thread.Sleep(10000);
            commander.Ping();
        }
    }
}
