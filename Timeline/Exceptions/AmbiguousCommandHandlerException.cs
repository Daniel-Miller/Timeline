using System;

namespace Timeline.Exceptions
{
    internal class AmbiguousCommandHandlerException : Exception
    {
        public AmbiguousCommandHandlerException(string name)
            : base($"You cannot define multiple handlers for the same command ({name}).")
        {
        }
    }
}