using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Files;

public class DeleteFileRoute : INatsuAPIRoute
{
    public string RoutePath => "/files/:id";
    public HttpMethod Method => HttpMethod.Delete;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetStringParameter("id", out var id))
            return;

        var file = TaggedFileHelper.Get(id);

        if (file is null)
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
