using Natsu.Backend.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Natsu.Backend.Components;

public static class ImageComparer
{
    public static float CalculateSimilarity(TaggedFile first, TaggedFile second)
    {
        using var prevA = loadPreview(first);
        using var prevB = loadPreview(second);

        var valA = calculateValue(prevA);
        var valB = calculateValue(prevB);

        var val = 0;

        for (int i = 0; i < valA.Length; i++)
        {
            var a = valA[i];
            var b = valB[i];

            var diff = Math.Abs(a - b);
            val += 255 - diff;
        }

        return (float)val / (valA.Length * 255);
    }

    private static Image<Rgba32> loadPreview(TaggedFile file)
    {
        var path = FileManager.GetPathFor(file.PreviewHash);
        return Image.Load<Rgba32>(path);
    }

    private static byte[] calculateValue(Image<Rgba32> image)
    {
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Size = new Size(64),
            Mode = ResizeMode.Stretch
        }));

        image.Mutate(x => x.Grayscale());

        var list = new List<byte>();

        for (int x = 0; x < image.Width; x++)
        {
            for (int y = 0; y < image.Height; y++)
            {
                list.Add(image[x, y].R);
            }
        }

        return list.ToArray();
    }
}
