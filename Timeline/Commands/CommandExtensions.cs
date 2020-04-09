using System;

using Timeline.Utilities;

namespace Timeline.Commands
{
    /// <summary>
    /// Provides functions to convert between instances of ICommand and SerializedCommand.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Returns a deserialized command.
        /// </summary>
        public static ICommand Deserialize(this SerializedCommand x, ISerializer serializer)
        {
            var data = serializer.Deserialize<ICommand>(x.CommandData, Type.GetType(x.CommandClass));

            data.AggregateIdentifier = x.AggregateIdentifier;
            data.ExpectedVersion = x.ExpectedVersion;

            data.IdentityTenant = x.IdentityTenant;
            data.IdentityUser = x.IdentityUser;

            return data;
        }

        /// <summary>
        /// Returns a serialized command.
        /// </summary>
        public static SerializedCommand Serialize(this ICommand command, ISerializer serializer, Guid aggregateIdentifier, int? version)
        {
            var data = serializer.Serialize(command, new[] { "AggregateIdentifier", "AggregateVersion", "IdentityTenant", "IdentityUser", "CommandIdentifier", "SendScheduled", "SendStarted", "SendCompleted", "SendCancelled" });

            var serialized = new SerializedCommand
            {
                AggregateIdentifier = aggregateIdentifier,
                ExpectedVersion = version,

                CommandClass = command.GetType().AssemblyQualifiedName,
                CommandType = command.GetType().Name,
                CommandData = data,

                CommandIdentifier = command.CommandIdentifier,

                IdentityTenant = command.IdentityTenant,
                IdentityUser = command.IdentityUser
            };

            if (serialized.CommandClass.Length > 200)
                throw new OverflowException($"The assembly-qualified name for this command ({serialized.CommandClass}) exceeds the maximum character limit (200).");

            if (serialized.CommandType.Length > 100)
                throw new OverflowException($"The type name for this command ({serialized.CommandType}) exceeds the maximum character limit (100).");

            if ((serialized.SendStatus?.Length ?? 0) > 20)
                throw new OverflowException($"The send status ({serialized.SendStatus}) exceeds the maximum character limit (20).");

            return serialized;
        }
    }
}