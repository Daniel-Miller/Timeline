using System;
using System.Collections.Generic;

using Timeline.Utilities;

namespace Timeline.Commands
{
    /// <summary>
    /// Defines the methods needed from the command store.
    /// </summary>
    public interface ICommandStore
    {
        /// <summary>
        /// Utility for serializing and deserializing commands.
        /// </summary>
        ISerializer Serializer { get; }

        /// <summary>
        /// Returns true if a command exists.
        /// </summary>
        bool Exists(Guid command);

        /// <summary>
        /// Gets the serialized version of specific command.
        /// </summary>
        SerializedCommand Get(Guid command);

        /// <summary>
        /// Gets all unstarted commands that are scheduled to send now.
        /// </summary>
        IEnumerable<SerializedCommand> GetExpired(DateTimeOffset at);

        /// <summary>
        /// Saves a serialized command.
        /// </summary>
        void Save(SerializedCommand command, bool isNew);

        /// <summary>
        /// Returns the serialized version of a command.
        /// </summary>
        SerializedCommand Serialize(ICommand command);
    }
}
