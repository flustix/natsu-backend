using Newtonsoft.Json;

namespace Natsu.Backend;

[JsonObject(MemberSerialization.OptIn)]
public class Config
{
    [JsonProperty("mongo-str")]
    public string MongoString { get; set; } = "mongodb://localhost:27017";

    [JsonProperty("mongo-db")]
    public string MongoDatabase { get; set; } = "natsu";

    [JsonProperty("data-path")]
    public string DataPath { get; set; } = ".";

    [JsonProperty("port")]
    public int Port { get; set; } = 6510;

    [JsonProperty("ffmpeg")]
    public string FfmpegPath { get; init; } = "ffmpeg";
}
