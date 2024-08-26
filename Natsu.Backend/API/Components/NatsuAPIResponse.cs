using Midori.API.Components.Json;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Components;

public class NatsuAPIResponse : JsonResponse
{
    [JsonProperty("errors")]
    public Dictionary<string, string>? Errors { get; set; }
}
