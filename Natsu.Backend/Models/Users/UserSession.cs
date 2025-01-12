using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Natsu.Backend.Models.Users;

public class UserSession
{
    [BsonId]
    public ObjectId ID { get; init; } = ObjectId.GenerateNewId();

    [BsonElement("user")]
    public ObjectId UserID { get; set; } = ObjectId.Empty;

    [BsonElement("token")]
    public string Token { get; set; } = string.Empty;
}
