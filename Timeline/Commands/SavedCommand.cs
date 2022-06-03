using System;

namespace Timeline.Commands
{
    /// <summary>
    /// Provides a serialization wrapper for commands so that common properties are not embedded inside the command data.
    /// </summary>
    public class SavedCommand
    {
        public SavedCommand(ICommand command)
        {
            Command = command;
        }

        public ICommand Command { get; set; }

        public DateTimeOffset? SendScheduled { get; set; }
        public DateTimeOffset? SendStarted { get; set; }
        public DateTimeOffset? SendCompleted { get; set; }
        public DateTimeOffset? SendCancelled { get; set; }

        public string SendStatus { get; set; }
        public string SendError { get; set; }
    }
}