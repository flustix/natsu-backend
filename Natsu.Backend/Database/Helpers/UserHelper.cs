using MongoDB.Bson;
using MongoDB.Driver;
using Natsu.Backend.Authentication;
using Natsu.Backend.Models.Users;

namespace Natsu.Backend.Database.Helpers;

public class UserHelper
{
    private static IMongoCollection<User> users => MongoDatabase.GetCollection<User>("users");

    public static List<User> All => users.Find(_ => true).ToList();

    public static User Add(string username, string password)
    {
        var user = new User
        {
            ID = ObjectId.GenerateNewId(),
            Username = username,
            Password = PasswordAuth.HashPassword(password)
        };

        users.InsertOne(user);
        return user;
    }

    public static User? Get(ObjectId id) => users.Find(u => u.ID == id).FirstOrDefault();
    public static User? GetByUsername(string name) => users.Find(u => u.Username.Equals(name, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
}
