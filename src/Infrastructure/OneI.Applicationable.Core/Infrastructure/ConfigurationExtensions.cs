namespace OneI.Applicationable.Infrastructure;

using Microsoft.Extensions.Configuration;

internal static class ConfigurationExtensions
{
    /// <summary>
    /// Gets the immediate children sub-sections of configuration root based on key.
    /// </summary>
    /// <param name="root">Configuration from which to retrieve sub-sections.</param>
    /// <param name="path">Key of a section of which children to retrieve.</param>
    /// <returns>Immediate children sub-sections of section specified by key.</returns>
    internal static IEnumerable<IConfigurationSection> GetChildrenImplementation(this IConfigurationRoot root, string? path)
    {
        using var reference = (root as Internal.ConfigurationManager)?.GetProvidersReference();
        var providers = reference?.Providers ?? root.Providers;

        var children = providers
            .Aggregate(Enumerable.Empty<string>(),
                (seed, source) => source.GetChildKeys(seed, path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(key => root.GetSection(path == null ? key : ConfigurationPath.Combine(path, key)));

        if(reference is null)
        {
            return children;
        }
        else
        {
            // Eagerly evaluate the IEnumerable before releasing the reference so we don't allow iteration over disposed providers.
            return children.ToList();
        }
    }
}
