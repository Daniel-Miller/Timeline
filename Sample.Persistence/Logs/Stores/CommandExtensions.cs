using System;
using Timeline.Commands;

namespace Sample.Persistence.Logs.Stores
{
    /// <summary>
    /// Provides functions to convert between instances of ICommand and SerializedCommand.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Returns a deserialized command.
        /// </summary>
        public static SavedCommand Deserialize(this SerializedCommand serializedCommand)
        {
            var serializer = new Serializer();
            var data = serializer.Deserialize<ICommand>(serializedCommand.CommandData, Type.GetType(serializedCommand.CommandClass));

            data.AggregateIdentifier = serializedCommand.AggregateIdentifier;
            data.ExpectedVersion = serializedCommand.ExpectedVersion;
            data.CommandIdentifier = serializedCommand.CommandIdentifier;
            data.IdentityTenant = serializedCommand.IdentityTenant;
            data.IdentityUser = serializedCommand.IdentityUser;

            var savedCommand = new SavedCommand(data)
            {
                SendStatus = serializedCommand.SendStatus,
                SendError = serializedCommand.SendError,

                SendScheduled = serializedCommand.SendScheduled,
                SendStarted = serializedCommand.SendStarted,
                SendCompleted = serializedCommand.SendCompleted,
                SendCancelled = serializedCommand.SendCancelled,
            };

            return savedCommand;
        }

        /// <summary>
        /// Returns a serialized command.
        /// </summary>
        public static SerializedCommand Serialize(this SavedCommand commandSchedule)
        {
            var serializer = new Serializer();
            var data = serializer.Serialize(commandSchedule.Command, new[] { "AggregateIdentifier", "AggregateVersion", "IdentityTenant", "IdentityUser", "CommandIdentifier", "SendScheduled", "SendStarted", "SendCompleted", "SendCancelled" });

            var serialized = new SerializedCommand
            {
                AggregateIdentifier = commandSchedule.Command.AggregateIdentifier,
                ExpectedVersion = commandSchedule.Command.ExpectedVersion,

                CommandClass = serializer.GetClassName(commandSchedule.Command.GetType()),
                CommandType = commandSchedule.Command.GetType().Name,
                CommandData = data,

                CommandIdentifier = commandSchedule.Command.CommandIdentifier,

                IdentityTenant = commandSchedule.Command.IdentityTenant,
                IdentityUser = commandSchedule.Command.IdentityUser,

                SendStatus = commandSchedule.SendStatus,
                SendError = commandSchedule.SendError,

                SendScheduled = commandSchedule.SendScheduled,
                SendStarted = commandSchedule.SendStarted,
                SendCompleted = commandSchedule.SendCompleted,
                SendCancelled = commandSchedule.SendCancelled,
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