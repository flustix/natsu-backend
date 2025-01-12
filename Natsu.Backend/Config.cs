using Newtonsoft.Json;

namespace Natsu.Backend;

[JsonObject(MemberSerialization.OptIn)]
public class Config
{
    public string ServerName { get; set; } = Environment.MachineName;
    public string DataPath { get; set; } = "/files";
    public string FfmpegPath { get; init; } = "ffmpeg";
    public string FfprobePath { get; init; } = "ffprobe";
}
