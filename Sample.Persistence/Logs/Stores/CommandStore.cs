using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

using Timeline.Commands;
using Timeline.Utilities;

namespace Sample.Persistence.Logs
{
    public class CommandStore : ICommandStore
    {
        private string DatabaseConnectionString { get; set; }

        public ISerializer Serializer { get; private set; }

        public CommandStore(ISerializer serializer, string databaseConnectionString)
        {
            Serializer = serializer;
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

        public SerializedCommand Get(Guid command)
        {
            using (var db = new LogDbContext(DatabaseConnectionString))
            {
                var entity = db.Commands
                    .AsNoTracking()
                    .Where(x => x.CommandIdentifier == command)
                    .FirstOrDefault();

                return entity ?? throw new CommandNotFoundException($"Command not found: {command}");
            }
        }

        public IEnumerable<SerializedCommand> GetExpired(DateTimeOffset at)
        {
            using (var db = new LogDbContext(DatabaseConnectionString))
            {
                var commands = db.Commands
                    .AsNoTracking()
                    .Where(x => x.SendScheduled <= at && x.SendStatus == "Scheduled")
                    .ToArray();

                return commands;
            }
        }

        public void Save(SerializedCommand command, bool isNew)
        {
            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                if (isNew)
                    InsertCommand(command, connection);
                else
                    UpdateCommand(command, connection);
            }
        }

        public SerializedCommand Serialize(ICommand command)
        {
            return command.Serialize(Serializer, command.AggregateIdentifier, command.ExpectedVersion);
        }

        #region Methods (insert and update)

        private void InsertCommand(SerializedCommand c, SqlConnection connection)
        {
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

                parameters.AddWithValue("AggregateIdentifier", c.AggregateIdentifier);
                parameters.AddWithValue("ExpectedVersion", c.ExpectedVersion ?? (object)DBNull.Value);

                parameters.AddWithValue("CommandIdentifier", c.CommandIdentifier);
                parameters.AddWithValue("CommandClass", c.CommandClass);
                parameters.AddWithValue("CommandType", c.CommandType);
                parameters.AddWithValue("CommandData", c.CommandData);

                parameters.AddWithValue("IdentityTenant", c.IdentityTenant);
                parameters.AddWithValue("IdentityUser", c.IdentityUser);

                parameters.AddWithValue("SendStatus", (object)c.SendStatus ?? DBNull.Value);
                parameters.AddWithValue("SendError", (object)c.SendError ?? DBNull.Value);

                parameters.AddWithValue("SendScheduled", (object)c.SendScheduled ?? DBNull.Value);
                parameters.AddWithValue("SendStarted", (object)c.SendStarted ?? DBNull.Value);
                parameters.AddWithValue("SendCompleted", (object)c.SendCompleted ?? DBNull.Value);
                parameters.AddWithValue("SendCancelled", (object)c.SendCancelled ?? DBNull.Value);

                try
                {
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex) { throw new SqlInsertException($"The command ({c.CommandType}) could not be saved.", ex); }
            }
        }

        private void UpdateCommand(SerializedCommand c, SqlConnection connection)
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

                parameters.AddWithValue("CommandIdentifier", c.CommandIdentifier);

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
                    throw new Exception($"The command ({c.CommandType}) could not be saved.", ex);
                }
            }
        }

        #endregion
    }
}
