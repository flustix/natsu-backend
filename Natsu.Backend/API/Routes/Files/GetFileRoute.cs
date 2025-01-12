using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Utils;

namespace Natsu.Backend.API.Routes.Files;

public class GetFileRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files/:id";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetStringParameter("id", out var id))
            return;

        var path = interaction.GetStringQuery("path") ?? "";
        path = path.FormatPath();

        var file = id == "path" ? TaggedFileHelper.GetByPath(path, interaction.UserID) : TaggedFileHelper.Get(id);

        if (file is null || file.Owner != interaction.UserID)
        {
            await interaction.ReplyError(HttpStatusCode.NotFound, "File not found.");
            return;
        }

        await interaction.Reply(HttpStatusCode.OK, file);
    }
}
