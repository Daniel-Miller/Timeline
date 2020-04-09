using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Diagnostics;

using Humanizer;

namespace Sample.Presentation.Console
{
    public static class Bootstrap
    {
        static RootCommand ParseCommandLine(ProgramSettings settings)
        {
            var root = new RootCommand
            {
                new Option<int>(
                    "--tasks",
                    () => settings.TaskCount,
                    "Stress tests only: Number of parallel tasks to execute. Simulates concurrent users."),
                new Option<int>(
                    "--commands",
                    () => settings.CommandCount,
                    "Stress tests only: Number of commands to send per task. Simulates concurrent operations per user."),
                new Option<int>(
                    "--delay",
                    () => settings.MaxSleep,
                    "Stress tests only: Maximum number of milliseconds to delay between commands per task. Simulates speed per user."),
                new Option<int>(
                    "--snapshot-interval",
                    () => settings.SnapshotInterval,
                    "Number of events between aggregate snapshots."),
                new Option<bool>(
                    "--save-all-commands",
                    () => settings.SaveAllCommands,
                    "Creates a log entry for all commands sent. Use False to save only scheduled commands.")
            };

            root.Description = "Timeline Sample Console Application";

            root.Parse();

            root.Handler = CommandHandler.Create<int, int, int, int, bool>((int tasks, int commands, int delay, int snapshotInterval, bool saveAllCommands) =>
            {
                settings.TaskCount = tasks;
                settings.CommandCount = commands;
                settings.MaxSleep = delay;
                settings.SnapshotInterval = snapshotInterval;
                settings.SaveAllCommands = saveAllCommands;

                var watch = Stopwatch.StartNew();

                System.Console.WriteLine($"\n{root.Description}\n");

                var program = new Program(settings);
                program.Execute();

                System.Console.WriteLine("\nSettings:");
                System.Console.WriteLine(" * " + "Task".ToQuantity(tasks));
                System.Console.WriteLine(" * " + "Command".ToQuantity(commands));
                System.Console.WriteLine(" * " + "Delay " + "millisecond".ToQuantity(delay) + " between commands");
                System.Console.WriteLine(" * " + $"Snapshot after every {snapshotInterval} events");
                System.Console.WriteLine(" * " + $"Save {(saveAllCommands ? "all" : "only scheduled")} commands");

                System.Console.WriteLine();
                System.Console.WriteLine($"  Throughput: {Throughput(watch, tasks * commands)}");
                System.Console.WriteLine($"Elapsed Time: {ElapsedTime(watch)}");
            });

            return root;
        }

        static int Main(string[] args)
        {
            var settings = new ProgramSettings("Timeline", "OfflineStoragePath");
            return ParseCommandLine(settings).Invoke(args);
        }

        private static string ElapsedTime(Stopwatch watch)
        {
            return TimeSpan.FromMilliseconds(watch.ElapsedMilliseconds).Humanize(3);
        }

        private static string Throughput(Stopwatch watch, int operations)
        {
            var x = operations / ((double)watch.ElapsedMilliseconds / 1000d);
            return $"{x:n0} operations per second";
        }
    }
}
