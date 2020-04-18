using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using Timeline.Events;
using Timeline.Identities;
using Timeline.Utilities;

namespace Sample.Persistence.Logs
{
    public class EventStore : IEventStore
    {
        private string DatabaseConnectionString { get; set; }

        private string OfflineStorageFolder { get; set; }

        private readonly IIdentityService _identityService;

        public ISerializer Serializer { get; private set; }

        public EventStore(IIdentityService identityService, ISerializer serializer, string databaseConnectionString, string offlineStorageFolder)
        {
            _identityService = identityService;
            Serializer = serializer;
            DatabaseConnectionString = databaseConnectionString;
            OfflineStorageFolder = offlineStorageFolder;
        }

        public void Box(Guid aggregate)
        {
            GetClassAndTenant(aggregate, out string aggregateClass, out Guid aggregateTenant);

            // Create a new directory using the aggregate identifier as the folder name.
            var path = Path.Combine(OfflineStorageFolder, aggregate.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Serialize the event stream and write it to an external file.
            var events = GetSerialized(aggregate, -1);
            var json = Serializer.Serialize(events);
            var file = Path.Combine(path, "Events.json");
            File.WriteAllText(file, json, Encoding.Unicode);
            var info = new FileInfo(file);

            // Delete the aggregate and the events from the online logs.
            var deleted = Delete(aggregate);

            // Create a metadata file to describe the boxed aggregated.
            var meta = new StringBuilder();
            meta.AppendLine($"Aggregate Identifier : {aggregate}");
            meta.AppendLine($"     Aggregate Class : {aggregateClass}");
            meta.AppendLine($"    Aggregate Tenant : {aggregateTenant}");
            meta.AppendLine($"  Serialized Events  : {events.Count():n0}");
            meta.AppendLine($"Deleted Log Entries  : {deleted:n0}");
            meta.AppendLine($"    Date/Time Boxed  : {DateTime.Now:dddd, MMMM d, yyyy HH:mm} Local Time");
            meta.AppendLine($"                     : {DateTimeOffset.UtcNow:dddd, MMMM d, yyyy HH:mm} UTC");
            file = Path.Combine(path, "Metadata.txt");
            File.WriteAllText(file, meta.ToString());

            // Write an index entry for the boxed aggregate.
            var index = Path.Combine(OfflineStorageFolder, "Boxes.csv");
            File.AppendAllText(index, $"{DateTime.Now:yyyy/MM/dd-HH:mm},{aggregate},{aggregateClass},{info.Length / 1024} KB,{aggregateTenant}\n");
        }

        public bool Exists(Guid aggregate)
        {
            const string query = @"SELECT TOP 1 1 FROM logs.Aggregate WHERE AggregateIdentifier = @AggregateIdentifier";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();
                using (var select = new SqlCommand(query, connection))
                {
                    select.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    object o = select.ExecuteScalar();
                    return o != DBNull.Value;
                }
            }
        }

        public bool Exists(Guid aggregate, int version)
        {
            const string query = @"SELECT TOP 1 1 FROM logs.Aggregate WHERE AggregateIdentifier = @AggregateIdentifier AND AggregateVersion = @AggregateVersion";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();
                using (var select = new SqlCommand(query, connection))
                {
                    select.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    select.Parameters.AddWithValue("AggregateVersion", version);
                    object o = select.ExecuteScalar();
                    return o != DBNull.Value;
                }
            }
        }

        public IEnumerable<IEvent> Get(Guid aggregate, int fromVersion)
        {
            return GetSerialized(aggregate, fromVersion)
                .Select(x => x.Deserialize(Serializer))
                .ToList()
                .AsEnumerable();
        }

        public IEnumerable<Guid> GetExpired(DateTimeOffset at)
        {
            using (var db = new LogDbContext(DatabaseConnectionString))
            {
                return db.Aggregates
                    .AsNoTracking()
                    .Where(x => x.AggregateExpires != null && x.AggregateExpires <= at)
                    .Select(x => x.AggregateIdentifier)
                    .ToList();
            }
        }

        private IEnumerable<SerializedEvent> GetSerialized(Guid aggregate, int fromVersion)
        {
            const string text = @"
SELECT 
    AggregateIdentifier,
    AggregateVersion,
    EventTime,
    EventClass,
    EventType,
    EventData,
    IdentityTenant,
    IdentityUser
FROM 
    logs.Event 
WHERE
    AggregateIdentifier = @AggregateIdentifier AND AggregateVersion > @AggregateVersion
ORDER BY
    AggregateVersion";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(text, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    command.Parameters.AddWithValue("AggregateVersion", fromVersion);

                    using (var reader = command.ExecuteReader())
                    {
                        var list = new List<SerializedEvent>();

                        while (reader.Read())
                        {
                            var item = new SerializedEvent
                            {
                                AggregateIdentifier = reader.GetGuid(0),
                                AggregateVersion = reader.GetInt32(1),
                                EventTime = reader.GetDateTimeOffset(2),
                                EventClass = reader.GetString(3),
                                EventType = reader.GetString(4),
                                EventData = reader.GetString(5),
                                IdentityTenant = reader.GetGuid(6),
                                IdentityUser = reader.GetGuid(7)
                            };
                            list.Add(item);
                        }

                        return list;
                    }
                }
            }
        }

        public IEvent Last(Guid aggregate)
        {
            return GetSerialized(aggregate, -1)
                .Last()
                .Deserialize(Serializer);
        }

        public void Save(AggregateRoot aggregate, IEnumerable<IEvent> events)
        {
            var current = _identityService.GetCurrent();
            var tenant = current.Tenant.Identifier;
            var user = current.User.Identifier;

            var list = new List<SerializedEvent>();

            foreach (var e in events)
            {
                var item = e.Serialize(Serializer, aggregate.AggregateIdentifier, e.AggregateVersion, tenant, user);

                list.Add(item);
            }

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    EnsureAggregateExists(tenant, aggregate.AggregateIdentifier, aggregate.GetType().Name.Replace("Aggregate", string.Empty), aggregate.GetType().FullName, connection, transaction);

                    if (list.Count > 1)
                        InsertEvents(list, connection, transaction);
                    else
                        InsertEvent(list[0], connection, transaction);

                    transaction.Commit();
                }
            }
        }

        #region Methods (insert, update, delete)

        private int Delete(Guid aggregate)
        {
            const string query = @"
DELETE FROM logs.Aggregate WHERE AggregateIdentifier = @AggregateIdentifier;
DELETE FROM logs.Event WHERE AggregateIdentifier = @AggregateIdentifier;
";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    return command.ExecuteNonQuery();
                }
            }
        }

        private void InsertEvent(SerializedEvent e, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"
INSERT INTO logs.Event
(
    AggregateIdentifier, AggregateVersion,
    EventClass, EventType, EventData,
    IdentityTenant, IdentityUser,
    EventTime
)
VALUES
( @AggregateIdentifier, @AggregateVersion, @EventClass, @EventType, @EventData, @IdentityTenant, @IdentityUser, @EventTime )";

            using (var command = new SqlCommand(query, connection, transaction))
            {
                var parameters = command.Parameters;

                parameters.AddWithValue("AggregateIdentifier", e.AggregateIdentifier);
                parameters.AddWithValue("AggregateVersion", e.AggregateVersion);

                parameters.AddWithValue("EventClass", e.EventClass);
                parameters.AddWithValue("EventType", e.EventType);
                parameters.AddWithValue("EventData", e.EventData);

                parameters.AddWithValue("IdentityTenant", e.IdentityTenant);
                parameters.AddWithValue("IdentityUser", e.IdentityUser);

                parameters.AddWithValue("EventTime", e.EventTime);

                try
                {
                    command.ExecuteNonQuery();
                }
                catch (SqlException ex) { throw new SqlInsertException($"The event ({e.EventType}) could not be saved.", ex); }
            }
        }

        private void InsertEvents(List<SerializedEvent> events, SqlConnection connection, SqlTransaction transaction)
        {
            var table = new DataTable();

            table.Columns.Add("AggregateIdentifier", typeof(Guid));
            table.Columns.Add("AggregateVersion", typeof(int));

            table.Columns.Add("IdentityTenant", typeof(Guid));
            table.Columns.Add("IdentityUser", typeof(Guid));

            table.Columns.Add("EventTime", typeof(DateTimeOffset));
            table.Columns.Add("EventClass", typeof(string));
            table.Columns.Add("EventType", typeof(string));
            table.Columns.Add("EventData", typeof(string));

            foreach (var e in events)
            {
                var row = table.NewRow();
                row["AggregateIdentifier"] = e.AggregateIdentifier;
                row["AggregateVersion"] = e.AggregateVersion;
                row["IdentityTenant"] = e.IdentityTenant;
                row["IdentityUser"] = e.IdentityUser;
                row["EventTime"] = e.EventTime;
                row["EventClass"] = e.EventClass;
                row["EventType"] = e.EventType;
                row["EventData"] = e.EventData;
                table.Rows.Add(row);
            }

            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
            {
                bulkCopy.BatchSize = 5000;
                bulkCopy.DestinationTableName = "logs.Event";
                bulkCopy.WriteToServer(table);
            }
        }

        #endregion

        #region Methods (lookup)

        private void EnsureAggregateExists(Guid tenant, Guid aggregate, string name, string type, SqlConnection connection, SqlTransaction transaction)
        {
            const string query = @"
IF NOT EXISTS(SELECT TOP 1 1 FROM logs.Aggregate WHERE AggregateIdentifier = @AggregateIdentifier)
  BEGIN
    INSERT INTO logs.Aggregate (TenantIdentifier, AggregateIdentifier, AggregateType, AggregateClass) VALUES (@TenantIdentifier, @AggregateIdentifier, @AggregateType, @AggregateClass);
  END";

            using (var insert = new SqlCommand(query, connection, transaction))
            {
                insert.Parameters.AddWithValue("TenantIdentifier", tenant);
                insert.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                insert.Parameters.AddWithValue("AggregateType", name);
                insert.Parameters.AddWithValue("AggregateClass", type);

                insert.ExecuteNonQuery();
            }
        }

        private void GetClassAndTenant(Guid aggregate, out string @class, out Guid tenant)
        {
            @class = null;
            tenant = Guid.Empty;

            const string text = @"select AggregateClass, TenantIdentifier from logs.Aggregate where AggregateIdentifier = @AggregateIdentifier";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(text, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            @class = reader.GetString(0);
                            tenant = reader.GetGuid(1);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
