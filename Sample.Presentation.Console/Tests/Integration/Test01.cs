using System;
using Sample.Application.Write;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    /// <summary>
    /// Scenario A: How to create and update a contact
    /// </summary>
    public static class Test01
    {
        public static void Run(ICommandQueue commander)
        {
            var alice = Guid.NewGuid();
            commander.Send(new RegisterPerson(alice, "Alice", "O'Wonderland"));
            commander.Send(new RenamePerson(alice, "Alice", "Cooper"));
        }
    }
}
