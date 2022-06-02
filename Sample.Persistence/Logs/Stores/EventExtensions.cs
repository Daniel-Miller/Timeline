using System;
using Timeline.Utilities;
using Timeline.Events;

namespace Sample.Persistence.Logs.Stores
{
    /// <summary>
    /// Provides functions to convert between instances of IEvent and SerializedEvent.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Returns a deserialized event.
        /// </summary>
        public static IEvent Deserialize(this SerializedEvent x)
        {
            var serializer = new Serializer();
            var data = serializer.Deserialize<IEvent>(x.EventData, Type.GetType(x.EventClass));

            data.AggregateIdentifier = x.AggregateIdentifier;
            data.AggregateVersion = x.AggregateVersion;
            data.EventTime = x.EventTime;
            data.IdentityTenant = x.IdentityTenant;
            data.IdentityUser = x.IdentityUser;

            return data;
        }

        /// <summary>
        /// Returns a serialized event.
        /// </summary>
        public static SerializedEvent Serialize(this IEvent @event, Guid aggregateIdentifier, int version, Guid tenant, Guid user)
        {
            var serializer = new Serializer();
            var data = serializer.Serialize(@event, new[] { "AggregateIdentifier", "AggregateVersion", "EventTime", "IdentityTenant", "IdentityUser" });

            var serialized = new SerializedEvent
            {
                AggregateIdentifier = aggregateIdentifier,
                AggregateVersion = version,

                EventTime = @event.EventTime,
                EventClass = serializer.GetClassName(@event.GetType()),
                EventType = @event.GetType().Name,
                EventData = data,

                IdentityTenant = Guid.Empty == @event.IdentityTenant ? tenant : @event.IdentityTenant,
                IdentityUser = Guid.Empty == @event.IdentityUser ? user : @event.IdentityUser
            };

            @event.IdentityTenant = serialized.IdentityTenant;
            @event.IdentityUser = serialized.IdentityUser;

            return serialized;
        }
    }
}