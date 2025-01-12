using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Natsu.Backend.Models;

[JsonObject(MemberSerialization.OptIn)]
public class FileTag
{
    /// <summary>
    /// The ID of the tag.
    /// </summary>
    [BsonId]
    [JsonProperty("id")]
    public ObjectId ID { get; set; } = ObjectId.GenerateNewId();

    /// <summary>
    /// The name of the tag.
    /// </summary>
    [BsonElement("name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The parents of this tag. When searching the parent tag, file with this tag will also be included.
    /// </summary>
    [BsonElement("parents")]
    [JsonProperty("parents")]
    public List<ObjectId> Parents { get; set; } = new();

    /// <summary>
    /// The Owner of this tag.
    /// </summary>
    [BsonElement("owner")]
    [JsonProperty("owner")]
    public ObjectId Owner { get; set; } = ObjectId.Empty;
}
