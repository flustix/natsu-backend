using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Models;
using Natsu.Backend.Utils;

namespace Natsu.Backend.API.Routes.Files;

public class SearchFilesRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var query = interaction.GetStringQuery("q") ?? "";
        var limit = interaction.GetIntQuery("l") ?? 25;
        var offset = interaction.GetIntQuery("o") ?? 0;

        limit = Math.Clamp(limit, 1, 100);
        offset = Math.Max(offset, 0);

        var search = new SearchFilter(query);

        var all = TaggedFileHelper.OwnedBy(interaction.UserID);
        all.RemoveAll(x => !search.Match(x));
        all.Sort((a, b) => a.Created.CompareTo(b.Created));
        all.Reverse();

        var files = all.Skip(offset).Take(limit);
        await interaction.Reply(HttpStatusCode.OK, files);
    }

    private static bool matches(TaggedFile file, string query)
    {
        var words = query.Split(' ');

        var match = words.All(w =>
        {
            var word = w;
            var invert = false;

            if (word.StartsWith('!'))
            {
                word = word[1..];
                invert = true;
            }

            return file.Description.Contains(word) == !invert;
        });
        return match;
    }
}
