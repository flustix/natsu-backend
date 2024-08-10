using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Newtonsoft.Json;

namespace Natsu.Backend.API.Routes.Folders;

public class GetFolderContentRoute : INatsuAPIRoute
{
    public string RoutePath => "/folders";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var path = interaction.GetStringQuery("path") ?? "";
        path = path.ToLowerInvariant().TrimEnd('/');

        // this gotta be the dumbest way to do only get the folders in the parent
        var pathSlashes = path.Count(c => c == '/');

        if (string.IsNullOrWhiteSpace(path))
            path = "/";

        var all = TaggedFileHelper.All;
        var dirs = all.Select(f => f.Directory).Distinct();
        dirs = dirs.Where(d =>
        {
            var slashes = d.Count(c => c == '/');
            return d.StartsWith(path) && slashes == pathSlashes + 1 && d != path;
        });

        var files = all.Where(f => f.Directory == path);

        await interaction.Reply(HttpStatusCode.OK, new
        {
            dirs = dirs.Select(d => new Folder
            {
                Path = d,
                Name = Path.GetFileName(d)
            }),
            files
        });
    }

    public class Folder
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;

        [JsonProperty("path")]
        public string Path { get; set; } = null!;
    }
}
