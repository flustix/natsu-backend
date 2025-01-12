using MongoDB.Bson;
using MongoDB.Driver;
using Natsu.Backend.Models.Users;
using Natsu.Backend.Utils;

namespace Natsu.Backend.Database.Helpers;

public class SessionHelper
{
    private static IMongoCollection<UserSession> sessions => MongoDatabase.GetCollection<UserSession>("sessions");

    public static List<UserSession> All => sessions.Find(_ => true).ToList();

    public static UserSession Create(ObjectId id)
    {
        var token = GenerateToken();

        while (doesTokenExist(token))
            token = GenerateToken();

        var session = new UserSession
        {
            ID = ObjectId.GenerateNewId(),
            UserID = id,
            Token = token
        };

        sessions.InsertOne(session);
        return session;
    }

    public static UserSession? GetForUser(ObjectId id) => sessions.Find(x => x.UserID == id).FirstOrDefault();
    public static UserSession? GetByToken(string token) => sessions.Find(x => x.Token == token).FirstOrDefault();

    public static UserSession? Get(string id) => !ObjectId.TryParse(id, out var obj) ? null : Get(obj);
    public static UserSession? Get(ObjectId id) => sessions.Find(x => x.ID == id).FirstOrDefault();

    public static string GenerateToken()
        => RandomizeUtils.GenerateRandomString(32, CharacterType.AllOfIt);

    private static bool doesTokenExist(string token) => sessions.Find(s => s.Token == token).FirstOrDefault() != null;
}
