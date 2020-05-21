using System;
using System.Collections.Generic;
using System.Linq;

using Timeline.Exceptions;
using Timeline.Identities;

namespace Timeline.Commands
{
    /// <summary>
    /// Implements a basic command queue. The purpose of the queue is to route commands to subscriber methods; 
    /// validation of a command itself is the responsibility of its subscriber/handler.
    /// </summary>
    public class CommandQueue : ICommandQueue
    {
        /// <summary>
        /// A command's full class name is used as the key to find the method that handles it.
        /// </summary>
        readonly Dictionary<string, Action<ICommand>> _subscribers;

        /// <summary>
        /// In a multi-tenant system we need to allow an individual tenant to override/customize the handling of a 
        /// command. In this case the class name and the tenant identifier are used together as the unique key.
        /// </summary>
        readonly Dictionary<CommandOverrideKey, Action<ICommand>> _overriders;

        /// <summary>
        /// Scheduled commands must be stored. Unscheduled commands can be stored, but this is optional.
        /// </summary>
        readonly ICommandStore _store;

        /// <summary>
        /// Used to determine the identity (tenant and user) of the person or system submitting a command.
        /// </summary>
        readonly IIdentityService _service;

        /// <summary>
        /// True if all commands (scheduled and unscheduled) are logged to the command store. False if only scheduled 
        /// commands are logged.
        /// </summary>
        readonly bool _saveAll;

        /// <summary>
        /// Constructs the queue.
        /// </summary>
        public CommandQueue(ICommandStore store, IIdentityService service, bool saveAll = false)
        {
            _subscribers = new Dictionary<string, Action<ICommand>>();
            _overriders = new Dictionary<CommandOverrideKey, Action<ICommand>>();
            _store = store;
            _service = service;
            _saveAll = saveAll;
        }

        #region Methods (subscription)

        /// <summary>
        /// One and only one subscriber can register for each command. If a command is sent then it must have a handler.
        /// </summary>
        public void Subscribe<T>(Action<T> action) where T : ICommand
        {
            var name = typeof(T).AssemblyQualifiedName;

            if (_subscribers.Any(x => x.Key == name))
                throw new AmbiguousCommandHandlerException(name);

            _subscribers.Add(name, (command) => action((T)command));
        }

        /// <summary>
        /// Registers a tenant-specific custom handler for the command.
        /// </summary>
        public void Override<T>(Action<T> action, Guid tenant) where T : ICommand
        {
            var key = new CommandOverrideKey
            {
                CommandName = typeof(T).AssemblyQualifiedName,
                IdentityTenant = tenant
            };

            if (_overriders.Any(x => x.Key.CommandName == key.CommandName && x.Key.IdentityTenant == key.IdentityTenant))
                throw new AmbiguousCommandHandlerException(key.CommandName);

            _overriders.Add(key, (command) => action((T)command));
        }

        #endregion

        #region Methods (sending synchronous commands)

        /// <summary>
        /// Executes the command synchronously.
        /// </summary>
        public void Send(ICommand command)
        {
            Identify(command);

            SerializedCommand serialized = null;

            if (_saveAll)
            {
                serialized = _store.Serialize(command);
                serialized.SendStarted = DateTimeOffset.UtcNow;
            }

            Execute(command, command.GetType().AssemblyQualifiedName);

            if (_saveAll)
            {
                serialized.SendCompleted = DateTimeOffset.UtcNow;
                serialized.SendStatus = "Completed";
                _store.Save(serialized, true);
            }
        }

        #endregion

        #region Methods (scheduling asynchronous commands)

        /// <summary>
        /// Schedules the command for asynchronous execution.
        /// </summary>
        public void Schedule(ICommand command, DateTimeOffset at)
        {
            Identify(command);

            var serialized = _store.Serialize(command);
            serialized.SendScheduled = at;
            serialized.SendStatus = "Scheduled";
            _store.Save(serialized, true);
        }

        /// <summary>
        /// Wakes the command queue to check for pending scheduled commands. Executes all commands for which the timer
        /// is now elapsed.
        /// </summary>
        public void Ping()
        {
            var commands = _store.GetExpired(DateTimeOffset.UtcNow);
            foreach (var command in commands)
                Execute(command);
        }

        /// <summary>
        /// Forces a scheduled command to start.
        /// </summary>
        public void Start(Guid command)
        {
            Execute(_store.Get(command));
        }

        /// <summary>
        /// Cancels a scheduled command.
        /// </summary>
        public void Cancel(Guid command)
        {
            var serialized = _store.Get(command);
            serialized.SendCancelled = DateTimeOffset.UtcNow;
            serialized.SendStatus = "Cancelled";
            _store.Save(serialized, false);
        }

        /// <summary>
        /// Completes a scheduled command.
        /// </summary>
        public void Complete(Guid command)
        {
            var serialized = _store.Get(command);
            serialized.SendCompleted = DateTimeOffset.UtcNow;
            serialized.SendStatus = "Completed";
            _store.Save(serialized, false);
        }

        #endregion

        #region Methods (execution)

        /// <summary>
        /// Invokes the subscriber method registered to handle the command.
        /// </summary>
        private void Execute(ICommand command, string @class)
        {
            if (_overriders.Keys.Any(k => k.CommandName == @class && k.IdentityTenant == command.IdentityTenant))
            {
                var customization = _overriders
                    .First(kv => kv.Key.CommandName == @class & kv.Key.IdentityTenant == command.IdentityTenant)
                    .Value;

                customization.Invoke(command);
            }
            else if (_subscribers.ContainsKey(@class))
            {
                var action = _subscribers[@class];
                action.Invoke(command);
            }
            else
            {
                throw new UnhandledCommandException(@class);
            }
        }

        /// <summary>
        /// Executes the command synchronously.
        /// </summary>
        private void Execute(SerializedCommand serialized)
        {
            serialized.SendStarted = DateTimeOffset.UtcNow;
            serialized.SendStatus = "Started";
            _store.Save(serialized, false);

            Execute(serialized.Deserialize(_store.Serializer), serialized.CommandClass);

            serialized.SendCompleted = DateTimeOffset.UtcNow;
            serialized.SendStatus = "Completed";
            _store.Save(serialized, false);
        }

        /// <summary>
        /// Determines the identity of the current user and sets the Identity properties on the command.
        /// </summary>
        private void Identify(ICommand command)
        {
            var current = _service.GetCurrent();
            var tenant = current.Tenant.Identifier;
            var user = current.User.Identifier;

            command.IdentityTenant = Guid.Empty == command.IdentityTenant ? tenant : command.IdentityTenant;
            command.IdentityUser = Guid.Empty == command.IdentityUser ? user : command.IdentityUser;
        }

        #endregion
    }
}
