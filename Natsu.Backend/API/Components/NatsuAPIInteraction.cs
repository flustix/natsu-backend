using System.Diagnostics.CodeAnalysis;
using System.Net;
using Midori.API.Components;
using Midori.API.Components.Json;
using Midori.Utils;

namespace Natsu.Backend.API.Components;

public class NatsuAPIInteraction : JsonInteraction<NatsuAPIResponse>
{
    protected override string[] AllowedMethods => base.AllowedMethods.Concat(extra_methods).ToArray();

    private static readonly string[] extra_methods = { "PATCH" };

    private Dictionary<string, object>? cache;
    private Dictionary<string, string>? errors;

    public void AddError(string field, string reason)
    {
        errors ??= new Dictionary<string, string>();
        errors[field] = reason;
    }

    public void AddCache(string key, object obj)
    {
        cache ??= new Dictionary<string, object>();
        cache[key] = obj;
    }

    public bool TryGetCache<T>(string key, [NotNullWhen(true)] out T? obj)
    {
        obj = default;

        if (cache is null)
            return false;

        try
        {
            obj = (T)cache[key];
            return obj != null;
        }
        catch
        {
            return false;
        }
    }

    public async void ReplyNothing(int code)
    {
        Response.StatusCode = code;
        await ReplyData(Array.Empty<byte>());
    }

    protected override Task ReplyJson(NatsuAPIResponse response)
    {
        response.Errors = errors;
        return base.ReplyJson(response);
    }

    public override Task HandleRoute<T>(IAPIRoute<T> route)
    {
        if (route is INatsuAPIRoute nr)
        {
            var errs = nr.Validate(this).ToList();

            if (errs.Count == 0)
                return base.HandleRoute(route);

            foreach (var (field, reason) in errs)
                AddError(field, reason);

            return ReplyError(HttpStatusCode.BadRequest, "InvalidRequest");
        }

        throw new InvalidOperationException($"Expected {typeof(INatsuAPIRoute)} but received {route.GetType()}.");
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
