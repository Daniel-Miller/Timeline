using System;
using System.Data.SqlClient;
using System.IO;
using System.Text;

using Timeline.Snapshots;

namespace Sample.Persistence.Logs
{
    public class SnapshotStore : ISnapshotStore
    {
        private string DatabaseConnectionString { get; set; }

        private string OfflineStorageFolder { get; set; }

        public SnapshotStore(string databaseConnectionString, string offlineStorageFolder)
        {
            DatabaseConnectionString = databaseConnectionString;
            OfflineStorageFolder = offlineStorageFolder;
        }

        public void Box(Guid aggregate)
        {
            // Create a new directory using the aggregate identifier as the folder name.
            var path = Path.Combine(OfflineStorageFolder, aggregate.ToString());
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Serialize the event stream and write it to an external file.
            var json = Get(aggregate).AggregateState;
            var file = Path.Combine(path, "Snapshot.json");
            File.WriteAllText(file, json, Encoding.Unicode);

            // Delete the aggregate and the events from the online logs.
            Delete(aggregate);
        }

        public Snapshot Get(Guid id)
        {
            const string text = @"
SELECT 
    AggregateIdentifier,
    AggregateVersion,
    AggregateState
FROM 
    logs.Snapshot 
WHERE
    AggregateIdentifier = @AggregateIdentifier";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(text, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Snapshot
                            {
                                AggregateIdentifier = reader.GetGuid(0),
                                AggregateVersion = reader.GetInt32(1),
                                AggregateState = reader.GetString(2),
                            };
                        }

                        return null;
                    }
                }
            }
        }

        public void Save(Snapshot snapshot)
        {

            const string query = @"
IF NOT EXISTS(SELECT TOP 1 1 FROM logs.Snapshot WHERE AggregateIdentifier = @AggregateIdentifier)
  INSERT INTO logs.Snapshot (AggregateIdentifier, AggregateVersion, AggregateState) VALUES (@AggregateIdentifier, @AggregateVersion, @AggregateState);
ELSE
  UPDATE logs.Snapshot SET AggregateVersion = @AggregateVersion, AggregateState = @AggregateState WHERE AggregateIdentifier = @AggregateIdentifier;
";
            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();

                using (var insert = new SqlCommand(query, connection))
                {
                    insert.Parameters.AddWithValue("AggregateIdentifier", snapshot.AggregateIdentifier);
                    insert.Parameters.AddWithValue("AggregateVersion", snapshot.AggregateVersion);
                    insert.Parameters.AddWithValue("AggregateState", snapshot.AggregateState);

                    insert.ExecuteNonQuery();
                }
            }

            // 
        }

        public Snapshot Unbox(Guid aggregate)
        {
            // The snapshot must exist!
            var file = Path.Combine(OfflineStorageFolder, aggregate.ToString(), "Snapshot.json");
            if (!File.Exists(file))
                throw new SnapshotNotFoundException(file);

            // Read the serialized JSON into a new snapshot and return it.
            return new Snapshot
            {
                AggregateIdentifier = aggregate,
                AggregateVersion = 1,
                AggregateState = File.ReadAllText(file, Encoding.Unicode)
            };
        }

        #region Methods (delete)

        private void Delete(Guid aggregate)
        {
            const string query = @"DELETE FROM logs.Snapshot WHERE AggregateIdentifier = @AggregateIdentifier;";

            using (var connection = new SqlConnection(DatabaseConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("AggregateIdentifier", aggregate);
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}