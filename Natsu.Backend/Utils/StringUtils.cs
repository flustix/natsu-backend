namespace Natsu.Backend.Utils;

public static class StringUtils
{
    public static string FormatPath(this string path)
    {
        // change from backslash to slash
        path = path.ToLowerInvariant().Replace("\\", "/");

        // remove double slashes
        while (path.Contains("//"))
            path = path.Replace("//", "/");

        // add leading
        if (!path.StartsWith('/'))
            path = '/' + path;

        // remove trailing
        path = path.TrimEnd('/');

        return path;
    }

    public static bool IsValidFilePath(this string path) => !path.Any(Path.GetInvalidFileNameChars().Contains);
    public static bool IsValidDirectoryPath(this string path) => !path.Any(Path.GetInvalidPathChars().Contains);
}
