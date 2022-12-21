namespace OneI.Applicationable.Internal;

using System;

[Obsolete]
internal interface IConfigureContainerAdapter
{
    void ConfigureContainer(ApplicationBuilderContext context, object containerBuilder);
}
