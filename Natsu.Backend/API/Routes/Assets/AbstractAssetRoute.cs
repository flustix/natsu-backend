using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Models;

namespace Natsu.Backend.API.Routes.Assets;

public abstract class AbstractAssetRoute : INatsuAPIRoute
{
    public abstract string RoutePath { get; }
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetStringParameter("id", out var id))
            return;

        var file = TaggedFileHelper.Get(id);

        if (file is null)
        {
            await interaction.Reply(HttpStatusCode.NotFound);
            return;
        }

        var path = FileManager.GetPathFor(GetHash(file));

        if (!File.Exists(path))
        {
            await interaction.Reply(HttpStatusCode.NotFound);
            return;
        }

        var bytes = await File.ReadAllBytesAsync(path);
        await SendResponse(interaction, file, bytes);
    }

    public abstract string GetHash(TaggedFile file);
    public abstract Task SendResponse(NatsuAPIInteraction interaction, TaggedFile file, byte[] content);
}
