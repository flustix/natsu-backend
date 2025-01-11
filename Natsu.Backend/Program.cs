using Midori.API;
using Midori.Logging;
using Midori.Utils;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database;
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

        MongoDatabase.Initialize(Config.MongoString, Config.MongoDatabase);

        if (args.Any(x => x.StartsWith("--test:")))
        {
            TestRunner.Run(args.First(x => x.StartsWith("--test:")).Split(':')[1]);
            return;
        }

        var server = new APIServer<NatsuAPIInteraction>();
        server.AddRoutesFromAssembly<INatsuAPIRoute>(typeof(Program).Assembly);
        server.Start(new[] { RuntimeUtils.IsDebugBuild ? $"http://localhost:{Config.Port}/" : $"http://+:{Config.Port}/" });

        Logger.Log("Finished starting!");
        await Task.Delay(-1);
    }

    private static void loadConfig()
    {
        var env = Environment.GetEnvironmentVariables();

        Config = new Config
        {
            ServerName = env["SERVER_NAME"]?.ToString() ?? Environment.MachineName,
            MongoString = env["MONGO_CONNECTION"]?.ToString() ?? "mongodb://localhost:27017",
            MongoDatabase = env["MONGO_DATABASE"]?.ToString() ?? "natsu",
            DataPath = env["DATA_PATH"]?.ToString() ?? "/files",
            FfmpegPath = env["FFMPEG_PATH"]?.ToString() ?? "ffmpeg"
        };
    }
}
