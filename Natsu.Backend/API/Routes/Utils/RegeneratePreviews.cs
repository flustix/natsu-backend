using System.Net;
using Midori.Logging;
using Natsu.Backend.API.Components;
using Natsu.Backend.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Utils;

public class RegeneratePreviews : INatsuAPIRoute
{
    public string RoutePath => "/utils/regenerate-previews";
    public HttpMethod Method => HttpMethod.Post;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var thread = new Thread(() =>
        {
            var files = TaggedFileHelper.All;

            foreach (var file in files)
            {
                var path = FileManager.GetPathFor(file.Hash);
                var content = File.ReadAllBytes(path);
                var hash = PreviewManager.CreatePreview(file, content);
                file.PreviewHash = hash;
                TaggedFileHelper.Update(file);
            }

            Logger.Log("Finished regenerating previews.");
        });

        thread.Start();
        await interaction.Reply(HttpStatusCode.Created);
    }
}
