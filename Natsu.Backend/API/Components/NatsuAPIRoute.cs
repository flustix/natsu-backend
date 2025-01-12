using JetBrains.Annotations;
using Midori.API.Components;

namespace Natsu.Backend.API.Components;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface INatsuAPIRoute : IAPIRoute<NatsuAPIInteraction>
{
    /// <summary>
    /// Validates whether the request is valid.
    /// <br />
    /// This is NOT for checking if a resource exists or not, just purely for the request body.
    /// <br />
    /// <br />
    /// To share data between validate and handle use <see cref="NatsuAPIInteraction.AddCache"/>
    /// and <see cref="NatsuAPIInteraction.TryGetCache{T}"/> from <see cref="NatsuAPIInteraction"/>.
    /// </summary>
    /// <param name="interaction">The interaction responsible for this request.</param>
    /// <returns>the list of errors (field, reason)</returns>
    IEnumerable<(string, string)> Validate(NatsuAPIInteraction interaction) => Array.Empty<(string, string)>();
}
