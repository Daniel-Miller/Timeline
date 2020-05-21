using System;

namespace Timeline.Exceptions
{
    internal class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(Type type, Guid id)
            : base($"This aggregate does not exist ({type.FullName} {id}) because there are no events for it.")
        {
        }
    }
}