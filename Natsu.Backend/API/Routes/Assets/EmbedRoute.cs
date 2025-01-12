using System.Net;
using System.Text;
using Natsu.Backend.API.Components;
using Natsu.Backend.Models;
using Natsu.Backend.Utils;

namespace Natsu.Backend.API.Routes.Assets;

public class EmbedRoute : AbstractAssetRoute
{
    public override string RoutePath => "/embed/:id";

    public override string GetHash(TaggedFile file) => file.PreviewHash;

    public override async Task SendResponse(NatsuAPIInteraction interaction, TaggedFile file, byte[] _)
    {
        var mime = file.MimeType;
        string template;

        if (mime.StartsWith("image/"))
            template = ResourceUtils.ReadStringFromAssembly("opengraph-image.html");
        else if (mime.StartsWith("video/"))
            template = ResourceUtils.ReadStringFromAssembly("opengraph-video.html");
        else
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, $"Unable to create embed for mime type '{mime}'.");
            return;
        }

        template = template.Replace("{{title}}", file.FileName);
        template = template.Replace("{{date}}", DateTime.Now.ToString("yyyy-MM-dd"));
        template = template.Replace("{{url}}", $"{Program.Config.BaseUrl}/assets/{file.ID}");
        template = template.Replace("{{mime}}", mime);

        var bytes = Encoding.UTF8.GetBytes(template);
        interaction.Response.ContentType = "text/html";
        await interaction.ReplyData(bytes);
    }
}
