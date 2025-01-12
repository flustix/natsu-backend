using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Utils;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes.Files;

public class EditFileRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files/:id";
    public HttpMethod Method => HttpMethod.Patch;

    public IEnumerable<(string, string)> Validate(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryParseBody<Payload>(out var payload))
            yield return ("_form", "Invalid json body.");

        if (payload is not null)
        {
            interaction.AddCache("payload", payload);

            if (!string.IsNullOrWhiteSpace(payload.Name) && !payload.Name.IsValidFilePath())
                yield return ("name", "Filename is not valid.");
            if (!string.IsNullOrWhiteSpace(payload.Folder) && !payload.Folder.IsValidDirectoryPath())
                yield return ("folder", "Directory name is not valid.");
        }
    }

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetCache<Payload>("payload", out var payload))
            throw new CacheMissingException("payload");

        if (!interaction.TryGetStringParameter("id", out var id))
            return;

        var file = TaggedFileHelper.Get(id);

        if (file is null || file.Owner != interaction.UserID)
        {
            await interaction.ReplyError(HttpStatusCode.NotFound, "File not found.");
            return;
        }

        if (payload.Description != null)
            file.Description = payload.Description;
        if (payload.Source != null)
            file.Source = payload.Source;

        if (!TaggedFileHelper.Update(file))
        {
            await interaction.ReplyError(HttpStatusCode.InternalServerError, "Failed to update file.");
            return;
        }

        await interaction.Reply(HttpStatusCode.OK, file);
    }

    public class Payload
    {
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
    }
}

