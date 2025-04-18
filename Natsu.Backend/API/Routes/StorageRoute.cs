﻿using System.Net;
using Natsu.Backend.API.Components;
using Natsu.Backend.Components;

namespace Natsu.Backend.API.Routes;

public class StorageRoute : INatsuAPIRoute
{
    public string RoutePath => "/storage";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        var path = Path.GetFullPath(FileManager.FilesPath);
        var root = Path.GetPathRoot(path)!;
        var drive = new DriveInfo(root);

        await interaction.Reply(HttpStatusCode.OK, new
        {
            used = drive.TotalSize - drive.AvailableFreeSpace,
            max = drive.TotalSize
        });
    }
}
