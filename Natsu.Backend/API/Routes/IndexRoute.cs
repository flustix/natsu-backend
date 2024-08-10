using Natsu.Backend.API.Components;

namespace Natsu.Backend.API.Routes;

public class IndexRoute : INatsuAPIRoute
{
    public string RoutePath => "/";
    public HttpMethod Method => HttpMethod.Get;

    public async Task Handle(NatsuAPIInteraction interaction)
        => await interaction.ReplyMessage("hi");
}
