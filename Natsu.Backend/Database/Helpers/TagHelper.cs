using MongoDB.Driver;
using Natsu.Backend.Models;

namespace Natsu.Backend.Database.Helpers;

public static class TagHelper
{
    private static IMongoCollection<FileTag> collection => MongoDatabase.GetCollection<FileTag>("tags");

    public static List<FileTag> All => collection.Find(_ => true).ToList();

    public static void Add(FileTag tag) => collection.InsertOne(tag);
}
