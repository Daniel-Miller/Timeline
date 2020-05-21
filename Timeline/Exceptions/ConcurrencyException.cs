using System;

namespace Timeline.Exceptions
{
    internal class ConcurrencyException : Exception
    {
        public ConcurrencyException(Guid aggregate)
            : base($"A concurrency violation occurred on this aggregate ({aggregate}). At least one event failed to save.")
        {
        }
    }
}