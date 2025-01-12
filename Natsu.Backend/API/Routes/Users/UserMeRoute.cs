using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Users;

public class UserMeRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/users/@me";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var user = UserHelper.Get(interaction.UserID);

        if (user is null)
        {
            await interaction.ReplyError(HttpStatusCode.NotFound, "User not found.");
            return;
        }

        await interaction.Reply(HttpStatusCode.OK, user);
    }
}
