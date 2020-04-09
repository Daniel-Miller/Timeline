using System;
using Humanizer;
using Sample.Application.Write;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    /// <summary>
    /// Scenario C: How to take an aggregate offline
    /// </summary>
    public static class Test03
    {
        public static void Run(ICommandQueue commander)
        {
            var hatter = Guid.NewGuid();
            commander.Send(new RegisterPerson(hatter, "Mad", "Hatter One"));
            for (int i = 2; i <= 8; i++)
                commander.Send(new RenamePerson(hatter, "Mad", "Hatter " + i.ToWords().Titleize()));
            commander.Send(new BoxPerson(hatter));
        }
    }
}
