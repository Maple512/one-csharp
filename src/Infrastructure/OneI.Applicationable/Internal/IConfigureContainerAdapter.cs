namespace OneI.Applicationable.Internal;

using System;
/// <summary>
/// The configure container adapter.
/// </summary>

[Obsolete]
internal interface IConfigureContainerAdapter
{
    /// <summary>
    /// Configures the container.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="containerBuilder">The container builder.</param>
    void ConfigureContainer(ApplicationBuilderContext context, object containerBuilder);
}
