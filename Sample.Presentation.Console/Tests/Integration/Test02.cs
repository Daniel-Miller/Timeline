using System;
using Humanizer;
using Sample.Application.Write;

using Timeline.Commands;

namespace Sample.Presentation.Console
{
    /// <summary>
    /// Scenario B: How to take a snapshot of an aggregate
    /// </summary>
    public static class Test02
    {
        public static void Run(ICommandQueue commander)
        {
            var henry = Guid.NewGuid();
            commander.Send(new RegisterPerson(henry, "King", "Henry I"));
            for (int i = 1; i <= 20; i++)
                commander.Send(new RenamePerson(henry, "King", "Henry " + (i+1).ToRoman()));
        }
    }
}
