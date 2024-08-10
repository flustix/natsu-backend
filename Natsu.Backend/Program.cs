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

        await loadConfig();

        MongoDatabase.Initialize(Config.MongoString, Config.MongoDatabase);

        if (args.Any(x => x.StartsWith("--test:")))
        {
            TestRunner.Run(args.First(x => x.StartsWith("--test:")).Split(':')[1]);
            return;
        }

        var server = new APIServer<NatsuAPIInteraction>();
        server.AddRoutesFromAssembly<INatsuAPIRoute>(typeof(Program).Assembly);
        server.Start(Config.Port);

        Logger.Log("Finished starting!");
        await Task.Delay(-1); // loop forever
    }

    private static async Task loadConfig()
    {
        if (!File.Exists("config.json"))
        {
            Logger.Log("Config file not found! Creating a new one...", LoggingTarget.General, LogLevel.Warning);
            await File.WriteAllTextAsync("config.json", new Config().Serialize(true));
        }

        Config = (await File.ReadAllTextAsync("config.json")).Deserialize<Config>() ?? throw new Exception("Failed to load config!");
    }
}
