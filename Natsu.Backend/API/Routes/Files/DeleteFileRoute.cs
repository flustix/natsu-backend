using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Files;

public class DeleteFileRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files/:id";
    public HttpMethod Method => HttpMethod.Delete;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetStringParameter("id", out var id))
            return;

        var file = TaggedFileHelper.Get(id);

        if (file is null || file.Owner != interaction.UserID)
        {
            await interaction.ReplyError(HttpStatusCode.NotFound, "File not found.");
            return;
        }

        var deleted = TaggedFileHelper.Delete(file.ID);

        if (deleted)
            await interaction.Reply(HttpStatusCode.NoContent);
        else
            await interaction.ReplyError(HttpStatusCode.NotModified, "Failed to delete.");
    }
}
