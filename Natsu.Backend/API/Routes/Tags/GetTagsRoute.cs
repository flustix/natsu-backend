using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Tags;

public class GetTagsRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/tags";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
        => await interaction.Reply(HttpStatusCode.OK, TagHelper.OwnedBy(interaction.UserID));
}
