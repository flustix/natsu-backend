namespace Natsu.Backend.Utils;

public static class MimeTypes
{
    private static readonly Dictionary<string, string> mime_type_map = new();

    static MimeTypes()
    {
        // image
        register("image/gif", "gif");
        register("image/jpeg", "jpeg", "jpg");
        register("image/png", "png");
        register("image/svg+xml", "svg");
        register("image/webp", "webp");

        // audio
        register("audio/mpeg", "mp3");
        register("audio/ogg", "ogg");
        register("audio/wav", "wav");

        // video
        register("video/mp4", "mp4");
        register("video/ogg", "ogv");
        register("video/quicktime", "mov", "qt");
        register("video/webm", "webm");
        register("video-x-matroska", "mkv");
    }

    private static void register(string mimeType, params string[] extensions)
    {
        foreach (var extension in extensions)
            mime_type_map[extension] = mimeType;
    }

    public static bool IsImage(string mime) => mime.StartsWith("image/");
    public static bool IsVideo(string mime) => mime.StartsWith("video/");

    public static string FromExtension(string ext)
        => mime_type_map.GetValueOrDefault(ext, "application/octet-stream");
}
