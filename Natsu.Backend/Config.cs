using Newtonsoft.Json;

namespace Natsu.Backend;

[JsonObject(MemberSerialization.OptIn)]
public class Config
{
    public string BaseUrl { get; set; } = "";
    public string ServerName { get; set; } = Environment.MachineName;
}
