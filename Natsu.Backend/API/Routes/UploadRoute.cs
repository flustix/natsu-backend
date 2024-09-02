using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Utils;
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

        var valid = true;

        if (string.IsNullOrEmpty(payload.Content))
        {
            interaction.AddError("content", "Content can not be empty.");
            valid = false;
        }

        if (string.IsNullOrWhiteSpace(payload.Name))
        {
            interaction.AddError("name", "Filename can not be empty.");
            valid = false;
        }

        if (!valid)
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, "Invalid form.");
            return;
        }

        var folder = payload.Folder ?? "";
        var path = Path.Combine(folder, payload.Name!).FormatPath();

        // check if already exists
        if (TaggedFileHelper.GetByPath(path) != null)
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, "This path is already used.");
            return;
        }

        byte[] bytes;

        try
        {
            if (payload.Content!.Contains(','))
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
            file.Source = payload.Source ?? "";
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

        [JsonProperty("source")]
        public string? Source { get; set; }

        [JsonProperty("folder")]
        public string? Folder { get; set; }

        [JsonProperty("nsfw")]
        public bool? NotSafeForWork { get; set; }

        [JsonProperty("created")]
        public long? CreationDate { get; set; }
    }
}
