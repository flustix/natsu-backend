using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes.Users;

public class UserCreateRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/users";
    public HttpMethod Method => HttpMethod.Post;

    public IEnumerable<(string, string)> Validate(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryParseBody<Payload>(out var payload))
            yield return ("_form", "Invalid json body.");

        if (payload is not null)
        {
            interaction.AddCache("payload", payload);

            if (string.IsNullOrEmpty(payload.Username))
                yield return ("username", "Username can not be empty.");
            if (string.IsNullOrEmpty(payload.Password))
                yield return ("password", "Password can not be empty.");
        }
    }

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetCache<Payload>("payload", out var payload))
            throw new CacheMissingException("payload");

        if (!interaction.User.IsAdmin)
        {
            await interaction.Reply(HttpStatusCode.Forbidden, "You are not allowed to create users.");
            return;
        }

        if (UserHelper.GetByUsername(payload.Username!) is not null)
        {
            await interaction.Reply(HttpStatusCode.Conflict, "User already exists.");
            return;
        }

        var user = UserHelper.Add(payload.Username!, payload.Password!);
        await interaction.Reply(HttpStatusCode.Created, user);
    }

    public class Payload
    {
        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }
    }
}
