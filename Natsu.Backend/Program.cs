using Midori.API;
using Midori.Logging;
using Midori.Utils;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Tests;
using Natsu.Backend.Utils.Json;

namespace Natsu.Backend;

public static class Program
{
    public static Config Config { get; private set; } = null!;

    public static async Task Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) => Logger.Error((Exception)eventArgs.ExceptionObject, "Unhandled exception!");
        JsonUtils.Converters.Add(new JsonObjectIdConverter());

        loadConfig();
        setupDatabase();

        if (args.Any(x => x.StartsWith("--test:")))
        {
            TestRunner.Run(args.First(x => x.StartsWith("--test:")).Split(':')[1]);
            return;
        }

        var server = new APIServer<NatsuAPIInteraction>();
        server.AddRoutesFromAssembly<INatsuAPIRoute>(typeof(Program).Assembly);
        server.Start(new[] { $"http://{(RuntimeUtils.IsDebugBuild ? "localhost" : "+")}:6510/" });

        Logger.Log("Finished starting!");
        await Task.Delay(-1);
    }

    private static void loadConfig()
    {
        var env = Environment.GetEnvironmentVariables();

        Config = new Config
        {
            BaseUrl = env["BASE_URL"]?.ToString() ?? "http://localhost:6510",
            ServerName = env["SERVER_NAME"]?.ToString() ?? Environment.MachineName,
        };
    }

    private static void setupDatabase()
    {
        var host = RuntimeUtils.IsDebugBuild ? "localhost" : "mongo";
        MongoDatabase.Initialize($"mongodb://{host}:27017", "natsu");

        if (UserHelper.All.Count == 0)
        {
            Logger.Log("No users found, creating default user...");

            var name = Environment.GetEnvironmentVariable("DEFAULT_USER");
            var pass = Environment.GetEnvironmentVariable("DEFAULT_PASS");

            if (string.IsNullOrWhiteSpace(name))
                Logger.Log("No default user found, using 'admin' as default username.", LoggingTarget.General, LogLevel.Warning);
            if (string.IsNullOrWhiteSpace(pass))
                Logger.Log("No default pass found, using 'admin' as default password.", LoggingTarget.General, LogLevel.Warning);

            var user = UserHelper.Add(name ?? "admin", pass ?? "admin", u => u.IsAdmin = true);

            Logger.Log("Default user created!");

            // migrate files and tags to the new user
            foreach (var file in TaggedFileHelper.All)
            {
                file.Owner = user.ID;
                TaggedFileHelper.Update(file);
            }

            foreach (var tag in TagHelper.All)
            {
                tag.Owner = user.ID;
                TagHelper.Update(tag);
            }
        }

        if (UserHelper.Admins.Count == 0)
        {
            var first = UserHelper.All.FirstOrDefault();

            if (first is null)
                throw new Exception("No users found!");

            first.IsAdmin = true;
            UserHelper.Update(first);
            Logger.Log($"Set user '{first.Username}' as admin.");
        }
    }
}
