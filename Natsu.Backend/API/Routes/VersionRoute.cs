using System.Net;
using Natsu.Backend.API.Components;

namespace Natsu.Backend.API.Routes;

public class VersionRoute : INatsuAPIRoute
{
    public string RoutePath => "/version";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
    {
        await interaction.Reply(HttpStatusCode.OK, new
        {
            version = GetType().Assembly.GetName().Version?.ToString(3) ?? "0.0.0",
            name = Program.Config.ServerName
        });
    }
}
