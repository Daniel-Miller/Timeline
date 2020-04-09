using System;
using System.Runtime.Serialization;

namespace Timeline.Exceptions
{
    [Serializable]
    internal class UnhandledEventException : Exception
    {
        public UnhandledEventException(string name)
            : base($"You must register at least one handler for this event ({name}).")
        {
        }

        protected UnhandledEventException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}