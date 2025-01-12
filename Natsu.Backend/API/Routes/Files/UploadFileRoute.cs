using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Utils;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes.Files;

public class UploadFileRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files";
    public HttpMethod Method => HttpMethod.Post;

    public IEnumerable<(string, string)> Validate(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryParseBody<Payload>(out var payload))
            yield return ("_form", "Invalid json body.");

        if (payload is not null)
        {
            interaction.AddCache("payload", payload);

            if (string.IsNullOrEmpty(payload.Content))
                yield return ("content", "Content can not be empty.");

            if (string.IsNullOrWhiteSpace(payload.Name))
                yield return ("name", "Filename can not be empty.");
            else if (!payload.Name.IsValidFilePath())
                yield return ("name", "Filename is not valid.");

            if (!string.IsNullOrWhiteSpace(payload.Folder) && !payload.Folder.IsValidDirectoryPath())
                yield return ("folder", "Directory name is not valid.");
        }
    }

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetCache<Payload>("payload", out var payload))
            throw new CacheMissingException("payload");

        var folder = payload.Folder ?? "";
        var path = Path.Combine(folder, payload.Name!).FormatPath();

        // check if already exists
        if (TaggedFileHelper.GetByPath(path, interaction.UserID) != null)
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
            file.Owner = interaction.UserID;
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
