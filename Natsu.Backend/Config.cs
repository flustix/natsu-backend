using Newtonsoft.Json;

namespace Natsu.Backend;

[JsonObject(MemberSerialization.OptIn)]
public class Config
{
    public string ServerName { get; set; } = Environment.MachineName;
    public string MongoString { get; set; } = "mongodb://localhost:27017";
    public string MongoDatabase { get; set; } = "natsu";
    public string DataPath { get; set; } = "/files";
    public int Port { get; set; } = 6510;
    public string FfmpegPath { get; init; } = "ffmpeg";
    public string FfprobePath { get; init; } = "ffprobe";
}
