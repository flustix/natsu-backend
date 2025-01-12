using System.Net;
using MongoDB.Bson;
using Natsu.Backend.API.Components;
using Natsu.Backend.Authentication;
using Natsu.Backend.Database.Helpers;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes.Auth;

public class LoginRoute : INatsuAPIRoute
{
    public string RoutePath => "/auth/login";
    public HttpMethod Method => HttpMethod.Post;

    public IEnumerable<(string, string)> Validate(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryParseBody<LoginRequest>(out var payload))
            yield return ("_form", "Invalid json body.");

        if (payload is not null)
        {
            if (string.IsNullOrEmpty(payload.Username))
                yield return ("username", "Username can not be empty.");
            if (string.IsNullOrEmpty(payload.Password))
                yield return ("password", "Password can not be empty.");

            var user = UserHelper.GetByUsername(payload.Username!);

            if (user is null)
                yield return ("username", "Invalid username.");
            else
                interaction.AddCache("id", user.ID);

            if (user is not null && !PasswordAuth.VerifyPassword(payload.Password!, user.Password))
                yield return ("password", "Invalid password.");
        }
    }

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetCache<ObjectId>("id", out var id))
            throw new CacheMissingException("id");

        var session = SessionHelper.GetForUser(id);
        session ??= SessionHelper.Create(id);
        await interaction.Reply(HttpStatusCode.OK, session.Token);
    }

    private class LoginRequest
    {
        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("password")]
        public string? Password { get; set; }
    }
}
