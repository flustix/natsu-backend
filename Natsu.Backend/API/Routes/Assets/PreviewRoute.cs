using Natsu.Backend.API.Components;
using Natsu.Backend.Models;

namespace Natsu.Backend.API.Routes.Assets;

public class PreviewRoute : AbstractAssetRoute
{
    public override string RoutePath => "/preview/:id";

    public override string GetHash(TaggedFile file) => file.PreviewHash;

    public override async Task SendResponse(NatsuAPIInteraction interaction, TaggedFile file, byte[] content)
    {
        interaction.Response.ContentType = "image/jpg";
        await interaction.ReplyData(content);
    }
}
