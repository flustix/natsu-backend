using MongoDB.Bson;
using MongoDB.Driver;
using Natsu.Backend.Models;

namespace Natsu.Backend.Database.Helpers;

public static class TaggedFileHelper
{
    private static IMongoCollection<TaggedFile> collection => MongoDatabase.GetCollection<TaggedFile>("files");

    public static List<TaggedFile> All => collection.Find(_ => true).ToList();

    public static void Add(TaggedFile file) => collection.InsertOne(file);

    public static TaggedFile? Get(string id) => !ObjectId.TryParse(id, out var obj) ? null : Get(obj);
    public static TaggedFile? Get(ObjectId id) => collection.Find(x => x.ID == id).FirstOrDefault();

    public static bool Delete(string id) => ObjectId.TryParse(id, out var obj) && Delete(obj);

    public static bool Delete(ObjectId id)
    {
        var result = collection.DeleteOne(f => f.ID == id);
        return result.DeletedCount > 0;
    }

    public static TaggedFile? GetByPath(string id) => collection.Find(x => x.FilePath == id).FirstOrDefault();
}
