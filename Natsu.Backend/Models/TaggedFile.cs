using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Natsu.Backend.Utils;
using Newtonsoft.Json;

namespace Natsu.Backend.Models;

[JsonObject(MemberSerialization.OptIn)]
public class TaggedFile
{
    [BsonId]
    [JsonProperty("id")]
    public ObjectId ID { get; set; } = ObjectId.GenerateNewId();

    [BsonElement("path")]
    [JsonProperty("path")]
    public string FilePath { get; set; } = string.Empty;

    [BsonIgnore]
    [JsonProperty("directory")]
    public string Directory => Path.GetDirectoryName(FilePath)?.Replace('\\', '/') ?? "/";

    [BsonIgnore]
    [JsonProperty("name")]
    public string FileName => Path.GetFileName(FilePath);

    [BsonIgnore]
    [JsonProperty("ext")]
    public string Extension => Path.GetExtension(FileName);

    [BsonIgnore]
    [JsonProperty("mime")]
    public string MimeType => FileUtils.ToMimeType(Extension.Replace(".", ""));

    [BsonElement("hash")]
    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;

    [BsonElement("preview-hash")]
    public string PreviewHash { get; set; } = string.Empty;

    [BsonElement("description")]
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("nsfw")]
    [JsonProperty("nsfw")]
    public bool NotSafeForWork { get; set; }

    [BsonElement("tags")]
    [JsonProperty("tags")]
    public List<ObjectId> Tags { get; set; } = new();

    [BsonElement("created")]
    [JsonProperty("created")]
    public long Created { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

    [BsonElement("modified")]
    [JsonProperty("modified")]
    public long Modified { get; set; } = DateTimeOffset.Now.ToUnixTimeSeconds();

    [BsonElement("size")]
    [JsonProperty("size")]
    public long Size { get; set; }
}
