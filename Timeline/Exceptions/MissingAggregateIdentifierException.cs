using System;
using System.Runtime.Serialization;

namespace Timeline.Exceptions
{
    [Serializable]
    internal class MissingAggregateIdentifierException : Exception
    {
        public MissingAggregateIdentifierException(Type aggregateType, Type eventType)
            : base($"The aggregate identifier is missing from both the aggregate instance ({aggregateType.FullName}) and the event instance ({eventType.FullName}).")
        {
        }

        protected MissingAggregateIdentifierException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}