using System;

namespace Timeline.Exceptions
{
    internal class UnhandledCommandException : Exception
    {
        public UnhandledCommandException(string name)
            : base($"There is no handler registered for this command ({name}). One handler (and only one handler) must be registered.")
        {
        }
    }
}