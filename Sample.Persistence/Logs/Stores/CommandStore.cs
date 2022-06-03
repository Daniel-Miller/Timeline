using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using Timeline.Commands;
using Timeline.Utilities;

namespace Sample.Persistence.Logs.Stores
{
    public class CommandStore : ICommandStore
    {
        private string DatabaseConnectionString { get; set; }

        public CommandStore(string databaseConnectionString)
        {
            DatabaseConnectionString = databaseConnectionString;
        }

        public void Delete(Guid id)
        {
            using (var db = new LogDbContext(DatabaseConnectionString))
            {
                var command = db.Commands.FirstOrDefault(x => x.CommandIdentifier == id);
                if (command != null)
                {
                    db.Commands.Remove(command);
                    db.SaveChanges();
                }
            }
        }

        public bool Exists(Guid command)
        {
            using (var db = new LogDbContext(DatabaseConnectionString))
            {
                return db.Commands
                    .AsNoTracking()
                    .Any(x => x.CommandIdentifier == command);
            }
        }

        public SavedCommand Get(Guid commandIdentifier)
        {
            using (var db = new LogDbContext(DatabaseConnectionString))
            {
                var entity = db.Commands
                    .AsNoTracking()
                    .Where(x => x.CommandIdentifier == commandIdentifier)
                    .FirstOrDefault();

                var command = entity?.Deserialize();

                return command ?? throw new CommandNotFoundException($"Command not found: {commandIdentifier}");
            }
        }

        public IEnumerable<SavedCommand> GetExpired(DateTimeOffset at)
        {
            using (var db = new LogDbContext(DatabaseConnectionString))
            {
                var commands = db.Commands
                    .AsNoTracking()
                    .Where(x => x.SendScheduled <= at && x.SendStatus == "Scheduled")
                    .Select(e => e.Deserialize())
                    .ToArray();

                return commands;
            }
        }

        public void Save(SavedCommand command, bool isNew)
        {
            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                if (isNew)
                    InsertCommand(command, connection);
                else
                    UpdateCommand(command, connection);
            }
        }

        #region Methods (insert and update)

        private void InsertCommand(SavedCommand c, SqlConnection connection)
        {
            var serializedCommand = c.Serialize();

            const string query = @"
INSERT INTO logs.Command
(
    AggregateIdentifier, ExpectedVersion,
    CommandIdentifier, CommandClass, CommandType, CommandData,
    IdentityTenant, IdentityUser,
    SendStatus, SendError,
    SendScheduled, SendStarted, SendCompleted, SendCancelled
)
VALUES
( @AggregateIdentifier, @ExpectedVersion, @CommandIdentifier, @CommandClass, @CommandType, @CommandData, @IdentityTenant, @IdentityUser, @SendStatus, @SendError, @SendScheduled, @SendStarted, @SendCompleted, @SendCancelled )";

            using (var command = new SqlCommand(query, connection))
            {
                var parameters = command.Parameters;

                parameters.AddWithValue("AggregateIdentifier", serializedCommand.AggregateIdentifier);
                parameters.AddWithValue("ExpectedVersion", serializedCommand.ExpectedVersion ?? (object)DBNull.Value);

                parameters.AddWithValue("CommandIdentifier", serializedCommand.CommandIdentifier);
                parameters.AddWithValue("CommandClass", serializedCommand.CommandClass);
                parameters.AddWithValue("CommandType", serializedCommand.CommandType);
                parameters.AddWithValue("CommandData", serializedCommand.CommandData);

                parameters.AddWithValue("IdentityTenant", serializedCommand.IdentityTenant);
                parameters.AddWithValue("IdentityUser", serializedCommand.IdentityUser);

                parameters.AddWithValue("SendStatus", (object)serializedCommand.SendStatus ?? DBNull.Value);
                parameters.AddWithValue("SendError", (object)serializedCommand.SendError ?? DBNull.Value);

                parameters.AddWithValue("SendScheduled", (object)serializedCommand.SendScheduled ?? DBNull.Value);
                parameters.AddWithValue("SendStarted", (object)serializedCommand.SendStarted ?? DBNull.Value);
                parameters.AddWithValue("SendCompleted", (object)serializedCommand.SendCompleted ?? DBNull.Value);
                parameters.AddWithValue("SendCancelled", (object)serializedCommand.SendCancelled ?? DBNull.Value);

                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex) { throw new SqlInsertException($"The command ({serializedCommand.CommandType}) could not be saved.", ex); }
            }
        }

        private void UpdateCommand(SavedCommand c, SqlConnection connection)
        {
            const string query = @"
UPDATE logs.Command
SET SendScheduled = @SendScheduled, SendStarted = @SendStarted, SendCompleted = @SendCompleted, SendCancelled = @SendCancelled, 
    SendStatus = @SendStatus, SendError = @SendError
WHERE CommandIdentifier = @CommandIdentifier
";

            using (var command = new SqlCommand(query, connection))
            {
                var parameters = command.Parameters;

                parameters.AddWithValue("CommandIdentifier", c.Command.CommandIdentifier);

                parameters.AddWithValue("SendScheduled", (object)c.SendScheduled ?? DBNull.Value);
                parameters.AddWithValue("SendStarted", (object)c.SendStarted ?? DBNull.Value);
                parameters.AddWithValue("SendCompleted", (object)c.SendCompleted ?? DBNull.Value);
                parameters.AddWithValue("SendCancelled", (object)c.SendCancelled ?? DBNull.Value);

                parameters.AddWithValue("SendStatus", (object)c.SendStatus ?? DBNull.Value);
                parameters.AddWithValue("SendError", (object)c.SendError ?? DBNull.Value);

                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    // TODO: Where to get the command name
                    // {/*c.CommandType*/}
                    throw new Exception($"The command () could not be saved.", ex);
                }
            }
        }

        #endregion
    }
}
