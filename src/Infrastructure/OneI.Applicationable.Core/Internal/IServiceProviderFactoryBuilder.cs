namespace OneI.Applicationable.Internal;

using Microsoft.Extensions.DependencyInjection;

internal interface IServiceProviderFactoryBuilder
{
    IServiceProvider Build(IApplicationBuilderContext context, IServiceCollection services);
}
