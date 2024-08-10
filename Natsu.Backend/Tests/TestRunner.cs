using Midori.Logging;
using Natsu.Backend.Components;

namespace Natsu.Backend.Tests;

public class TestRunner
{
    public static void Run(string test)
    {
        switch (test)
        {
            case "logger":
                testLogger();
                break;

            case "file":
                saveFile();
                break;

            default:
                throw new Exception("Unknown test!");
        }
    }

    private static void saveFile()
    {
        const string path = @"X:\homework\101545935_p0_master1200.jpg";

        var content = File.ReadAllBytes(path);
        FileManager.CreateFile(Path.GetFileName(path), content);
    }

    private static void testLogger()
    {
        Logger.Log("This is a test message!");
        Logger.Log("This is a test warning!", LoggingTarget.General, LogLevel.Warning);
        Logger.Log("This is a test debug message!", LoggingTarget.General, LogLevel.Debug);
        Logger.Log("This is a test error!", LoggingTarget.General, LogLevel.Error);

        Logger.Log("This is a test message!", LoggingTarget.Database);
        Logger.Log("This is a test warning!", LoggingTarget.Database, LogLevel.Warning);
        Logger.Log("This is a test debug message!", LoggingTarget.Database, LogLevel.Debug);
        Logger.Log("This is a test error!", LoggingTarget.Database, LogLevel.Error);

        Logger.Log("This is a test message!", LoggingTarget.Network);
        Logger.Log("This is a test warning!", LoggingTarget.Network, LogLevel.Warning);
        Logger.Log("This is a test debug message!", LoggingTarget.Network, LogLevel.Debug);
        Logger.Log("This is a test error!", LoggingTarget.Network, LogLevel.Error);

        Logger.Log("This is a test message which only shows in the console!", LoggingTarget.Info);
        Logger.Log("This is a test warning which only shows in the console!", LoggingTarget.Info, LogLevel.Warning);
        Logger.Log("This is a test debug message which only shows in the console!", LoggingTarget.Info, LogLevel.Debug);
        Logger.Log("This is a test error which only shows in the console!", LoggingTarget.Info, LogLevel.Error);
    }
}
