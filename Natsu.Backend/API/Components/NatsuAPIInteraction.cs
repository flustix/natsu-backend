using System.Diagnostics.CodeAnalysis;
using System.Net;
using Midori.API.Components;
using Midori.API.Components.Interfaces;
using Midori.API.Components.Json;
using Midori.Utils;
using MongoDB.Bson;
using Natsu.Backend.Database.Helpers;

namespace Natsu.Backend.API.Components;

public class NatsuAPIInteraction : JsonInteraction<NatsuAPIResponse>, IHasAuthorizationInfo
{
    protected override string[] AllowedMethods => base.AllowedMethods.Concat(extra_methods).ToArray();

    private static readonly string[] extra_methods = { "PATCH" };

    public ObjectId UserID { get; private set; } = ObjectId.Empty;

    public bool IsAuthorized => UserID != ObjectId.Empty;
    public string AuthorizationError { get; private set; } = string.Empty;

    private Dictionary<string, object>? cache;
    private Dictionary<string, string>? errors;

    protected override void OnPopulate()
    {
        base.OnPopulate();

        var token = Request.Headers["Authorization"];
        token ??= Request.Cookies["token"]?.Value;

        if (string.IsNullOrEmpty(token))
        {
            AuthorizationError = "No token provided.";
            return;
        }

        token = token.Split(" ").Last().Trim();
        var session = SessionHelper.GetByToken(token);

        if (session == null)
        {
            AuthorizationError = "Invalid token.";
            return;
        }

        var user = UserHelper.Get(session.UserID);

        if (user == null)
        {
            AuthorizationError = "User not found.";
            return;
        }

        UserID = session.UserID;
    }

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
