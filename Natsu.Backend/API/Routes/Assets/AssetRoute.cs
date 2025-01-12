using Natsu.Backend.API.Components;
using Natsu.Backend.Models;

namespace Natsu.Backend.API.Routes.Assets;

public class AssetRoute : AbstractAssetRoute
{
    public override string RoutePath => "/assets/:id";

    public override string GetHash(TaggedFile file) => file.Hash;

    public override async Task SendResponse(NatsuAPIInteraction interaction, TaggedFile file, byte[] content)
    {
        interaction.Response.ContentType = file.MimeType;
        await interaction.ReplyData(content);
    }
}
