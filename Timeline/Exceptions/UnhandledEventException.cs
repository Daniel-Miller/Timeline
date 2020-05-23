using System;

namespace Timeline.Exceptions
{
    internal class UnhandledEventException : Exception
    {
        public UnhandledEventException(string name)
            : base($"You must register at least one handler for this event ({name}).")
        {
        }
    }
}