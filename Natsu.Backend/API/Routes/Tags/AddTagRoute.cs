using System.Net;
using Midori.API.Components.Interfaces;
using MongoDB.Bson;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Models;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes.Tags;

public class AddTagRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/tags";
    public HttpMethod Method => HttpMethod.Post;

    public IEnumerable<(string, string)> Validate(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryParseBody<Payload>(out var payload))
            yield return ("_form", "Invalid json body.");

        if (payload is not null)
        {
            interaction.AddCache("payload", payload);

            if (string.IsNullOrEmpty(payload.Name))
                yield return ("name", "Name can not be empty.");
        }
    }

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetCache<Payload>("payload", out var payload))
            throw new CacheMissingException("payload");

        var tag = new FileTag
        {
            ID = ObjectId.GenerateNewId(),
            Name = payload.Name!,
            Owner = interaction.UserID
        };

        TagHelper.Add(tag);
        await interaction.Reply(HttpStatusCode.Created, tag);
    }

    private class Payload
    {
        [JsonProperty("name")]
        public string? Name { get; set; }
    }
}
