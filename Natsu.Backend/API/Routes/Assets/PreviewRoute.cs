using Natsu.Backend.API.Components;
using Natsu.Backend.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Assets;

public class PreviewRoute : INatsuAPIRoute
{
    public string RoutePath => "/preview/:id";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetStringParameter("id", out var id))
            return;

        if (string.IsNullOrEmpty(id))
        {
            interaction.ReplyNothing(404);
            return;
        }

        var file = TaggedFileHelper.Get(id);

        if (file is null)
        {
            interaction.ReplyNothing(404);
            return;
        }

        var hash = file.PreviewHash;

        if (string.IsNullOrEmpty(hash))
        {
            interaction.ReplyNothing(404);
            return;
        }

        var path = FileManager.GetPathFor(hash);

        if (!File.Exists(path))
        {
            interaction.ReplyNothing(404);
            return;
        }

        var bytes = await File.ReadAllBytesAsync(path);
        interaction.Response.ContentType = file.MimeType;
        await interaction.ReplyData(bytes);
    }
}
