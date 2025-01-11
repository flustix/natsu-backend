﻿using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Components;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Routes.Assets;

public class AssetRoute : INatsuAPIRoute
{
    public string RoutePath => "/assets/:id";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        if (!interaction.TryGetStringParameter("id", out var id))
            return;

        var file = TaggedFileHelper.Get(id);

        if (file is null)
        {
            await interaction.Reply(HttpStatusCode.NotFound);
            return;
        }

        var path = FileManager.GetPathFor(file.Hash);

        if (!File.Exists(path))
        {
            await interaction.Reply(HttpStatusCode.NotFound);
            return;
        }

        var bytes = await File.ReadAllBytesAsync(path);
        interaction.Response.ContentType = file.MimeType;
        await interaction.ReplyData(bytes);
    }
}
