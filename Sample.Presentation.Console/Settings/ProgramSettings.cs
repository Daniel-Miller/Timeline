using System;
using System.Configuration;
using System.IO;
using System.Linq;

using Sample.Application.Read;

namespace Sample.Presentation.Console
{
    public class ProgramSettings
    {
        public string DatabaseConnectionString { get; set; }
        public string OfflineStorageFolder { get; set; }

        public int TaskCount { get; set; } = 1;
        public int CommandCount { get; set; } = 10;
        public int MaxSleep { get; set; } = 10;
        public int SnapshotInterval { get; set; } = 100;
        public bool SaveAllCommands { get; set; } = false;

        public static Tenant CurrentTenant { get; set; } = Tenants.Acme;

        public ProgramSettings(string connectionStringName, string offlineStorageFolder)
        {
            DatabaseConnectionString = GetConnectionString(connectionStringName);
            OfflineStorageFolder = GetAppSetting(offlineStorageFolder);
            ValidateDirectory(OfflineStorageFolder);
        }

        private string GetAppSetting(string name)
        {
            var value = ConfigurationManager.AppSettings[name];
            if (string.IsNullOrEmpty(value))
                throw new AppSettingNotFoundException(name);
            return value;
        }

        private string GetConnectionString(string name)
        {
            var cs = ConfigurationManager.ConnectionStrings[name];
            if (cs == null)
                throw new ConnectionStringNotFoundException(name);
            return cs.ConnectionString;
        }

        private void ValidateDirectory(string path)
        {
            if (!IsFullPath(path) || !Directory.Exists(path))
                throw new DirectoryNotFoundException(path);
        }

        public static bool IsFullPath(string path)
        {
            return !string.IsNullOrWhiteSpace(path)
                && path.IndexOfAny(Path.GetInvalidPathChars().ToArray()) == -1
                && Path.IsPathRooted(path)
                && !Path.GetPathRoot(path).Equals(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal);
        }
    }
}
