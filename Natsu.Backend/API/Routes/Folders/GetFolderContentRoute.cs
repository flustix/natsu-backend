using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Folders;

public class GetFolderContentRoute : INatsuAPIRoute
{
    public string RoutePath => "/folders";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var path = interaction.GetStringQuery("path") ?? "/";

        if (string.IsNullOrWhiteSpace(path))
            path = "/";

        path = path.ToLowerInvariant();

        var all = TaggedFileHelper.All;
        var dirs = all.Select(f => f.Directory).Distinct();

        var files = all.Where(f => f.Directory == path);

        await interaction.Reply(HttpStatusCode.OK, new
        {
            dirs = dirs.Where(d => d.StartsWith(path) && d != path),
            files
        });
    }
}
