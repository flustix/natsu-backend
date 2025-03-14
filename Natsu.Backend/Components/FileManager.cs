﻿using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Models;
using Natsu.Backend.Utils;

namespace Natsu.Backend.Components;

public static class FileManager
{
    public static string FilesPath { get; }

    static FileManager()
    {
        if (Environment.GetEnvironmentVariable("DEBUG_FILES") is { } path)
            FilesPath = path;
        else
            FilesPath = "/files";
    }

    public static TaggedFile CreateFile(string path, byte[] content, Action<TaggedFile>? applyData = null)
    {
        var hash = Hashing.GetHash(content);
        writeFile(hash, content);

        var file = new TaggedFile
        {
            FilePath = path,
            Hash = hash,
            Size = content.Length
        };

        file.PreviewHash = PreviewManager.CreatePreview(file, content);

        applyData?.Invoke(file);

        TaggedFileHelper.Add(file);

        return file;
    }

    private static void writeFile(string hash, byte[] content)
    {
        using var ms = new MemoryStream(content);
        writeFile(hash, ms);
    }

    private static void writeFile(string hash, Stream content)
    {
        var path = GetPathFor(hash);
        var folder = Path.GetDirectoryName(path)!;

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        using var stream = File.Create(path);
        content.Position = 0;
        content.CopyTo(stream);
    }

    public static string GetPathFor(string hash) => Path.Combine(FilesPath, HashToPath(hash));
    public static string HashToPath(string hash) => Path.Combine(hash[..1], hash[..2], hash);
}
