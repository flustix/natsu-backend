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

        SearchFilter<TaggedFile> search;

        try
        {
            search = new SearchFilter<TaggedFile>(query);
        }
        catch (Exception ex)
        {
            await interaction.ReplyError(HttpStatusCode.BadRequest, ex.Message);
            return;
        }

        var all = search.Filter(TaggedFileHelper.OwnedBy(interaction.UserID));
        all.Sort((a, b) => a.Created.CompareTo(b.Created));
        all.Reverse();

        var files = all.Skip(offset).Take(limit);
        await interaction.Reply(HttpStatusCode.OK, files);
    }
}
