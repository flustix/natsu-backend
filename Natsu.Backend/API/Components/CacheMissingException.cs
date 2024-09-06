namespace Natsu.Backend.API.Components;

public class CacheMissingException : Exception
{
    public CacheMissingException(string key)
        : base($"Key '{key}' was missing in validation cache.")
    {
    }
}
