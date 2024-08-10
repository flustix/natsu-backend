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
        var path = Path.Combine(folder, payload.Name).Replace("\\", "/").ToLowerInvariant();

        while (path.Contains("//"))
            path = path.Replace("//", "/");

        if (!path.StartsWith('/'))
            path = '/' + path;

        byte[] bytes;

        try
        {
            if (payload.Content.Contains(','))
                payload.Content = payload.Content.Split(",")[1];

            bytes = Convert.FromBase64String(payload.Content);
        }
        catch (Exception ex)
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, ex.Message);
            return;
        }

        var taggedFile = FileManager.CreateFile(path, bytes, file =>
        {
            file.Created = file.Modified = payload.CreationDate ?? DateTimeOffset.Now.ToUnixTimeSeconds();
            file.NotSafeForWork = payload.NotSafeForWork ?? false;
            file.Description = payload.Description ?? "";
        });

        await interaction.Reply(HttpStatusCode.Created, taggedFile);
    }

    public class Payload
    {
        [JsonProperty("content")]
        public string? Content { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("description")]
        public string? Description { get; set; }

        [JsonProperty("folder")]
        public string? Folder { get; set; }

        [JsonProperty("nsfw")]
        public bool? NotSafeForWork { get; set; }

        [JsonProperty("created")]
        public long? CreationDate { get; set; }
    }
}
