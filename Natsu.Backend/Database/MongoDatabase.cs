using Midori.Logging;
using MongoDB.Driver;

namespace Natsu.Backend.Database;

public static class MongoDatabase
{
    public static Logger Logger { get; } = Logger.GetLogger(LoggingTarget.Database);

    private static IMongoDatabase db { get; set; } = null!;

    public static void Initialize(string connectionString, string database)
    {
        Logger.Add("Initializing connection...");

        var client = new MongoClient(connectionString);
        db = client.GetDatabase(database);

        Logger.Add("Connection initialized!");
    }

    public static IMongoCollection<T> GetCollection<T>(string name) => db.GetCollection<T>(name);
}
