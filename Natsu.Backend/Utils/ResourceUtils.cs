using System.Reflection;

namespace Natsu.Backend.Utils;

public static class ResourceUtils
{
    private static Assembly assembly { get; }
    private static string prefix { get; }

    static ResourceUtils()
    {
        assembly = typeof(ResourceUtils).Assembly;
        prefix = assembly.GetName().Name!;
    }

    public static string ReadStringFromAssembly(string file)
    {
        using var stream = ReadStreamFromAssembly(file);

        if (stream is null)
            throw new ArgumentNullException($"File {file} was not found in resources.");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    public static Stream? ReadStreamFromAssembly(string name)
    {
        name = "Resources/" + name;

        string[] split = name.Split('/');
        for (int i = 0; i < split.Length - 1; i++)
            split[i] = split[i].Replace('-', '_');

        return assembly.GetManifestResourceStream($@"{prefix}.{string.Join('.', split)}");
    }

    public static IEnumerable<string> GetAllFiles()
    {
        return assembly.GetManifestResourceNames().Select(n =>
        {
            n = n[(n.StartsWith(prefix, StringComparison.Ordinal) ? prefix.Length + 1 : 0)..];

            int lastDot = n.LastIndexOf('.');

            char[] chars = n.ToCharArray();

            for (int i = 0; i < lastDot; i++)
            {
                if (chars[i] == '.')
                    chars[i] = '/';
            }

            return new string(chars).Replace("Resources/", "");
        });
    }
}
