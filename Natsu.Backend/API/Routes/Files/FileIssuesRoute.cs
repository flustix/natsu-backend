using System.Net;
using Midori.API.Components.Interfaces;
using Natsu.Backend.API.Components;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Models;

namespace Natsu.Backend.API.Routes.Files;

/// <summary>
/// Returns all files with issues. (missing description, source, tags, etc.)
/// </summary>
public class FileIssuesRoute : INatsuAPIRoute, INeedsAuthorization
{
    public string RoutePath => "/files/issues";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var limit = interaction.GetIntQuery("l") ?? 25;
        var offset = interaction.GetIntQuery("o") ?? 0;
        var type = interaction.GetStringQuery("t") ?? "all";

        limit = Math.Clamp(limit, 1, 100);
        offset = Math.Max(offset, 0);

        var all = TaggedFileHelper.OwnedBy(interaction.UserID).Where(f => matches(f, type)).ToList();
        all.Sort((a, b) => a.Created.CompareTo(b.Created));
        all.Reverse();

        var files = all.Skip(offset).Take(limit);
        await interaction.Reply(HttpStatusCode.OK, files);
    }

    private static bool matches(TaggedFile file, string type)
    {
        var match = false;

        switch (type)
        {
            case "all":
                match |= string.IsNullOrWhiteSpace(file.Description);
                match |= string.IsNullOrWhiteSpace(file.Source);
                match |= file.Tags.Count == 0;
                break;

            case "description":
                match |= string.IsNullOrWhiteSpace(file.Description);
                break;

            case "source":
                match |= string.IsNullOrWhiteSpace(file.Source);
                break;

            case "tags":
                match |= file.Tags.Count == 0;
                break;
        }

        return match;
    }
}

