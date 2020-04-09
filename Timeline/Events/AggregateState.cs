using Timeline.Exceptions;

namespace Timeline.Events
{
    /// <summary>
    /// Represents the state (data) of an aggregate. A derived class should be a POCO (DTO/Packet) that includes a When
    /// method for each event type that changes its property values. Ideally, the property values for an instance of 
    /// this class should be modified only through its When methods.
    /// </summary>
    public abstract class AggregateState
    {
        public void Apply(IEvent @event)
        {
            var when = GetType().GetMethod("When", new[] { @event.GetType() });

            if (when == null)
                throw new MethodNotFoundException(GetType(), "When", @event.GetType());

            when.Invoke(this, new object[] { @event });
        }
    }
}
