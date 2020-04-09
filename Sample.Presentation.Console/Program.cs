using Sample.Presentation.Console.Settings;

namespace Sample.Presentation.Console
{
    public class Program
    {
        private ProgramSettings _settings;

        public Program(ProgramSettings settings)
        {
            _settings = settings;

            DependencyInjector.RegisterImplementations(settings.SnapshotInterval, settings.SaveAllCommands, settings.DatabaseConnectionString, settings.OfflineStorageFolder);
        }

        public void Execute()
        {
            var commander = ServiceLocator.CommandQueue;
            var querySearch = ServiceLocator.QuerySearch;

            /*
            Test01.Run(commander);

            Test02.Run(commander);

            Test03.Run(commander);

            new Test04(commander, querySearch).Run();

            Test05.Run(commander);

            new Test06(commander).Run();

            new Test07(commander).Run();

            Test08.Run(commander);
            */
            Test09.Run(commander, _settings.TaskCount, _settings.CommandCount, _settings.MaxSleep);
        }
    }
}
