using Sample.Application.Read;
using Sample.Application.Write;
using Sample.Persistence;
using Sample.Persistence.Logs;
using Sample.Persistence.Queries;
using Sample.Presentation.Console.Identities;

using Timeline.Commands;
using Timeline.Events;
using Timeline.Identities;
using Timeline.Snapshots;

namespace Sample.Presentation.Console.Settings
{
    public static class ServiceLocator
    {
        public static ICommandQueue CommandQueue { get; private set; }
        public static ICommandStore CommandStore { get; private set; }

        public static IEventQueue EventQueue { get; private set; }
        public static IEventStore EventStore { get; private set; }

        public static IIdentityService IdentityService { get; private set; }
        public static IEventRepository EventRepository { get; private set; }
        public static IEventRepository SnapshotRepository { get; private set; }

        public static IQueryStore QueryStore { get; private set; }
        public static IQuerySearch QuerySearch { get; private set; }

        public static void InitializeTimeline(int snapshotInterval, bool saveAllCommands, string databaseConnectionString, string offlineStorageFolder)
        {
            // Register the tenant and user identification service.

            IdentityService = new IdentityService();

            // Register the implementations for storing and sending commands.

            CommandStore = new CommandStore(new Serializer(), databaseConnectionString);
            CommandQueue = new CommandQueue(CommandStore, IdentityService, saveAllCommands);

            // Register the implementations for storing and publishing events.

            EventStore = new EventStore(IdentityService, new Serializer(), databaseConnectionString, offlineStorageFolder);
            EventRepository = new EventRepository(EventStore);
            SnapshotRepository = new SnapshotRepository(EventStore, EventRepository, new SnapshotStore(databaseConnectionString, offlineStorageFolder), new SnapshotStrategy(snapshotInterval));
            EventQueue = new EventQueue(new Serializer());
        }

        public static void InitializeApplication(string databaseConnectionString)
        {
            // Register query store and search services.

            QueryStore = new QueryStore(databaseConnectionString);
            QuerySearch = new QuerySearch(databaseConnectionString);

            // Register subscribers for application commands and domain events.

            _ = new AccountCommandSubscriber(CommandQueue, EventQueue, SnapshotRepository);
            _ = new AccountEventSubscriber(EventQueue, QueryStore, QuerySearch);

            _ = new PersonCommandSubscriber(CommandQueue, EventQueue, SnapshotRepository);
            _ = new PersonEventSubscriber(EventQueue, QueryStore);

            _ = new TransferCommandSubscriber(CommandQueue, EventQueue, SnapshotRepository);
            _ = new TransferEventSubscriber(EventQueue, QueryStore, QuerySearch);
            _ = new TransferProcessManager(CommandQueue, EventQueue, SnapshotRepository);

            _ = new UserCommandSubscriber(CommandQueue, EventQueue, SnapshotRepository);
            _ = new UserEventSubscriber(EventQueue, QueryStore);
            _ = new UserRegistrationProcessManager(CommandQueue, EventQueue, QuerySearch);
        }

        public static void InitializeCustomization()
        {
            // Register custom process managers for custom handlers and overrides.

            _ = new UmbrellaProcessManager(CommandQueue, EventQueue, QuerySearch);
        }
    }
}
