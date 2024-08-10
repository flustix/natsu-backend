using Midori.Logging;
using Natsu.Backend.Database.Helpers;
using Natsu.Backend.Models;
using Natsu.Backend.Utils;

namespace Natsu.Backend.Components;

public static class FileManager
{
    private static string folderPath => Path.Combine(Program.Config.DataPath, "files");

    public static TaggedFile CreateFile(string path, byte[] content, Action<TaggedFile>? applyData = null)
    {
        var hash = HashUtils.GetHash(content);
        writeFile(hash, content);

        var file = new TaggedFile
        {
            FilePath = path,
            Hash = hash,
            Size = content.Length,
            Created = DateTime.UtcNow,
            Modified = DateTime.UtcNow
        };

        file.PreviewHash = PreviewManager.CreatePreview(file, content);

        applyData?.Invoke(file);

        TaggedFileHelper.Add(file);
        Logger.Log($"File '{path}' created with hash '{hash}'!", LoggingTarget.General, LogLevel.Debug);

        return file;
    }

    private static void writeFile(string hash, byte[] content)
    {
        using var ms = new MemoryStream(content);
        writeFile(hash, ms);
    }

    private static void writeFile(string hash, Stream content)
    {
        Logger.Log($"Writing file with hash '{hash}'...", LoggingTarget.General, LogLevel.Debug);

        var path = GetPathFor(hash);
        var folder = Path.GetDirectoryName(path)!;

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        using var stream = File.Create(path);
        content.Position = 0;
        content.CopyTo(stream);

        Logger.Log($"{content.Length} bytes written!", LoggingTarget.General, LogLevel.Debug);
    }

    public static string GetPathFor(string hash) => Path.Combine(folderPath, HashToPath(hash));
    public static string HashToPath(string hash) => Path.Combine(hash[..1], hash[..2], hash);
}
