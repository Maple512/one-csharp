namespace OneI.Moduleable;

using Microsoft.Extensions.DependencyInjection;

public class MSService_Test
{
    [Fact]
    public void register_service()
    {
        var service = new ServiceCollection();

        _ = service.AddTransient<IService1, Service1>();

        var provider = service.BuildServiceProvider();
        _ = provider.GetService<IService1>();

        using var scope = provider.CreateScope();

        scope.ServiceProvider.GetService<IService1>().ShouldBeOfType<IService1>();
    }

    private interface IService1 { }

    private class Service1 : IService1 { }
}
