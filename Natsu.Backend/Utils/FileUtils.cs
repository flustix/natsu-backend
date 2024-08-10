namespace Natsu.Backend.Utils;

public class FileUtils
{
    private static readonly Dictionary<string, string> mime_type_map = new();

    static FileUtils()
    {
        // image
        registerMimeType("image/gif", "gif");
        registerMimeType("image/jpeg", "jpeg", "jpg");
        registerMimeType("image/png", "png");
        registerMimeType("image/svg+xml", "svg");
        registerMimeType("image/webp", "webp");

        // audio
        registerMimeType("audio/mpeg", "mp3");
        registerMimeType("audio/ogg", "ogg");
        registerMimeType("audio/wav", "wav");

        // video
        registerMimeType("video/mp4", "mp4");
        registerMimeType("video/ogg", "ogv");
        registerMimeType("video/quicktime", "mov", "qt");
        registerMimeType("video/webm", "webm");
        registerMimeType("video-x-matroska", "mkv");
    }

    private static void registerMimeType(string mimeType, params string[] extensions)
    {
        foreach (var extension in extensions)
            mime_type_map[extension] = mimeType;
    }

    public static bool IsImage(string mime) => mime.StartsWith("image/");
    public static bool IsVideo(string mime) => mime.StartsWith("video/");

    public static string ToMimeType(string extension)
        => mime_type_map.GetValueOrDefault(extension, "application/octet-stream");
}
