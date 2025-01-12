using MongoDB.Bson;
using MongoDB.Driver;
using Natsu.Backend.Models;

namespace Natsu.Backend.Database.Helpers;

public static class TagHelper
{
    private static IMongoCollection<FileTag> collection => MongoDatabase.GetCollection<FileTag>("tags");

    public static List<FileTag> All => collection.Find(_ => true).ToList();

    public static void Add(FileTag tag) => collection.InsertOne(tag);

    public static FileTag? Get(string id) => !ObjectId.TryParse(id, out var obj) ? null : Get(obj);
    public static FileTag? Get(ObjectId id) => collection.Find(x => x.ID == id).FirstOrDefault();

    public static List<FileTag> OwnedBy(ObjectId owner) => collection.Find(x => x.Owner == owner).ToList();

    public static void Update(FileTag tag) => collection.ReplaceOne(x => x.ID == tag.ID, tag);
}
