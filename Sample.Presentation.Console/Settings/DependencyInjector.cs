namespace Sample.Presentation.Console.Settings
{
    public static class DependencyInjector
    {
        public static void RegisterImplementations(int snapshotInterval, bool saveAllCommands, string databaseConnectionString, string offlineStorageFolder)
        {
            ServiceLocator.InitializeTimeline(snapshotInterval, saveAllCommands, databaseConnectionString, offlineStorageFolder);
            ServiceLocator.InitializeApplication(databaseConnectionString);
            ServiceLocator.InitializeCustomization();
        }
    }
}
