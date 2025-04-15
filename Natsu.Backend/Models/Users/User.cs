using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Natsu.Backend.Models.Users;

[JsonObject(MemberSerialization.OptIn)]
public class User
{
    [BsonId]
    public ObjectId ID { get; init; }

    [BsonElement("password")]
    public string Password { get; set; } = "";

    [BsonElement("username")]
    [JsonProperty("name")]
    public string Username { get; set; } = "";

    [BsonElement("avatar")]
    [JsonProperty("avatar")]
    public string? AvatarID { get; set; } = "";

    [BsonElement("admin")]
    [JsonProperty("admin")]
    public bool IsAdmin { get; set; }
}
