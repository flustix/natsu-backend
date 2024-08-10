using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Components;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes;

public class UploadRoute : INatsuAPIRoute
{
    public string RoutePath => "/upload";
    public HttpMethod Method => HttpMethod.Post;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryParseBody<Payload>(out var payload))
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, "invalid payload");
            return;
        }

        if (string.IsNullOrEmpty(payload.Content))
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, "Missing 'content' field.");
            return;
        }

        if (string.IsNullOrEmpty(payload.Name))
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, "Missing 'name' field.");
            return;
        }

        var folder = payload.Folder ?? "";
        var path = Path.Combine(folder, payload.Name).Replace("\\", "/");

        if (!path.StartsWith('/'))
            path = '/' + path;

        byte[] bytes;

        try
        {
            bytes = Convert.FromBase64String(payload.Content);
        }
        catch (Exception ex)
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, ex.Message);
            return;
        }

        var taggedFile = FileManager.CreateFile(path, bytes, file =>
        {
            file.Description = payload.Description ?? "";
        });

        await interaction.Reply(HttpStatusCode.Created, taggedFile);
    }

    private class Payload
    {
        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("folder")]
        public string? Folder { get; set; }
    }
}
