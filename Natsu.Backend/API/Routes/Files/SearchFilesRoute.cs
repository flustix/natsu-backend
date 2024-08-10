using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Models;

namespace Natsu.Backend.API.Routes.Files;

public class SearchFilesRoute : INatsuAPIRoute
{
    public string RoutePath => "/files";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var query = interaction.GetStringQuery("q") ?? "";
        var offset = interaction.GetIntQuery("o") ?? 0;

        var all = TaggedFileHelper.All;
        all.RemoveAll(x => !matches(x, query));

        var files = all.Skip(offset).Take(50);
        await interaction.Reply(HttpStatusCode.OK, files);
    }

    private static bool matches(TaggedFile file, string query)
    {
        var words = query.Split(' ');

        var match = words.All(w => file.Description.Contains(w));
        return match;
    }
}
