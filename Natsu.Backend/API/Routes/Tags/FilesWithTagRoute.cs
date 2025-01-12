using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Tags;

public class FilesWithTagRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/tags/:id/files";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetStringParameter("id", out var id))
            return;

        var tag = TagHelper.Get(id);

        if (tag is null || tag.Owner != interaction.UserID)
        {
            await interaction.Reply(HttpStatusCode.NotFound);
            return;
        }

        var files = TaggedFileHelper.OwnedBy(interaction.UserID);
        files = files.Where(x => x.Tags.Contains(tag.ID)).ToList();
        files.Sort((a, b) => a.Created.CompareTo(b.Created));
        await interaction.Reply(HttpStatusCode.OK, new { tag, files });
    }
}
