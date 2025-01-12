using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Files.Tags;

public class FileAddTagRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files/:f/tags/:t";
    public HttpMethod Method => HttpMethod.Put;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetStringParameter("f", out var fileID))
            return;

        if (!interaction.TryGetStringParameter("t", out var tagID))
            return;

        var file = TaggedFileHelper.Get(fileID);

        if (file is null || file.Owner != interaction.UserID)
        {
            await interaction.ReplyError(HttpStatusCode.NotFound, "File not found.");
            return;
        }

        var tag = TagHelper.Get(tagID);

        if (tag is null || tag.Owner != interaction.UserID)
        {
            await interaction.ReplyError(HttpStatusCode.NotFound, "Tag not found.");
            return;
        }

        if (file.Tags.Contains(tag.ID))
        {
            await interaction.Reply(HttpStatusCode.OK, file);
            return;
        }

        file.Tags.Add(tag.ID);
        TaggedFileHelper.Update(file);
        await interaction.Reply(HttpStatusCode.OK, file);
    }
}
