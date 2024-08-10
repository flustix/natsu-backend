using System.Diagnostics.CodeAnalysis;
using Midori.API.Components.Json;
using Midori.Utils;

namespace Natsu.Backend.API.Components;

public class NatsuAPIInteraction : JsonInteraction
{
    public async void ReplyNothing(int code)
    {
        Response.StatusCode = code;
        await ReplyData(Array.Empty<byte>());
    }

    public bool TryParseBody<T>([NotNullWhen(true)] out T? result)
    {
        result = default!;

        if (Request.InputStream == Stream.Null)
            return false;

        var body = new StreamReader(Request.InputStream).ReadToEnd();

        try
        {
            result = body.Deserialize<T>();
            return result != null;
        }
        catch
        {
            return false;
        }
    }
}
