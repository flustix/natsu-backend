using System.Diagnostics;
using Midori.Logging;
using Natsu.Backend.Models;
using Natsu.Backend.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Natsu.Backend.Components;

public class PreviewManager
{
    public static string CreatePreview(TaggedFile file, byte[] content)
    {
        try
        {
            var mime = file.MimeType;
            Logger.Log($"creating preview for {file.FilePath} ({mime})", LoggingTarget.General, LogLevel.Debug);

            if (FileUtils.IsImage(mime))
                return createImagePreview(content);
            if (FileUtils.IsVideo(mime))
                return createVideoPreview(content);
        }
        catch (Exception ex)
        {
            Logger.Error(ex, $"Failed to generate preview for '{file.FilePath}'!");
        }

        return "";
    }

    private static string createVideoPreview(byte[] content)
    {
        var videoPath = Path.GetTempFileName();
        var imagePath = Path.GetTempFileName().Replace(".tmp", ".png");

        // write temp file for ffmpeg
        var videoFs = File.Open(videoPath, FileMode.Create);
        videoFs.Write(content);
        videoFs.Dispose();

        var extractor = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = Program.Config.FfmpegPath,
                Arguments = $"-y -i \"{videoPath}\" -vf \"select=eq(n\\,0)\" -q:v 3 \"{imagePath}\"",
                CreateNoWindow = true,
                // RedirectStandardOutput = true,
                // RedirectStandardError = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            }
        };

        extractor.Start();
        extractor.WaitForExit();

        return createImagePreview(File.ReadAllBytes(imagePath));
    }

    private static string createImagePreview(byte[] content)
    {
        Logger.Log("creating image preview", LoggingTarget.General, LogLevel.Debug);
        using var original = Image.Load<Rgba32>(content);
        var smallest = Math.Min(original.Width, original.Height);

        using var copy = original.Clone();

        // skip if image is already small
        if (smallest > 256)
        {
            copy.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(256),
                Mode = ResizeMode.Crop
            }));
        }

        using var ms = new MemoryStream();
        copy.SaveAsJpeg(ms);

        ms.Seek(0, SeekOrigin.Begin);
        var hash = HashUtils.GetHash(ms);

        var path = FileManager.GetPathFor(hash);
        var folder = Path.GetDirectoryName(path)!;

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        using var fs = File.Open(path, FileMode.Create);
        ms.Seek(0, SeekOrigin.Begin);
        ms.CopyTo(fs);

        return hash;
    }
}
