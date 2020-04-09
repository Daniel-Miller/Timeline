using System;

namespace Timeline.Commands
{
    /// <summary>
    /// Provides a serialization wrapper for commands so that common properties are not embedded inside the command data.
    /// </summary>
    public class SerializedCommand : ICommand
    {
        public Guid AggregateIdentifier { get; set; }
        public int? ExpectedVersion { get; set; }

        public Guid IdentityTenant { get; set; }
        public Guid IdentityUser { get; set; }

        public string CommandClass { get; set; }
        public string CommandType { get; set; }
        public string CommandData { get; set; }

        public Guid CommandIdentifier { get; set; }

        public DateTimeOffset? SendScheduled { get; set; }
        public DateTimeOffset? SendStarted { get; set; }
        public DateTimeOffset? SendCompleted { get; set; }
        public DateTimeOffset? SendCancelled { get; set; }

        public string SendStatus { get; set; }
        public string SendError { get; set; }
    }
}