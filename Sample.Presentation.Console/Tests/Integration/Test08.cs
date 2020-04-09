using System;

using Sample.Application.Read;
using Sample.Application.Write;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    /// <summary>
    /// Scenario H: How to override a command with a custom handler
    /// </summary>
    public static class Test08
    {
        public static void Run(ICommandQueue commander)
        {
            ProgramSettings.CurrentTenant = Tenants.Umbrella;

            var alice = Guid.NewGuid();
            commander.Send(new RegisterPerson(alice, "Alice", "Abernathy"));
            commander.Send(new RenamePerson(alice, "Alice", "Parks"));
        }
    }
}
