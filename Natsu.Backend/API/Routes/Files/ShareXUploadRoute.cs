using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Utils;

namespace Natsu.Backend.API.Routes.Files;

public class ShareXUploadRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files/sharex";
    public HttpMethod Method => HttpMethod.Post;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetFile("file", out var file))
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, "Missing file.");
            return;
        }

        var path = Path.Combine("/", file.FileName).FormatPath();

        if (TaggedFileHelper.GetByPath(path, interaction.UserID) != null)
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, "This path is already used.");
            return;
        }

        byte[] bytes;

        try
        {
            var mem = new MemoryStream();
            await file.Data.CopyToAsync(mem);
            bytes = mem.ToArray();
        }
        catch (Exception ex)
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, ex.Message);
            return;
        }

        var taggedFile = FileManager.CreateFile(path, bytes, f =>
        {
            f.Owner = interaction.UserID;
            f.Created = DateTimeOffset.Now.ToUnixTimeSeconds();
        });

        await interaction.Reply(HttpStatusCode.Created, taggedFile);
    }
}
