using Microsoft.Extensions.Configuration;

namespace OneI.Applicationable.Configuration;

public interface IConfigurationManager : IConfigurationBuilder, IConfigurationRoot, IDisposable
{
}
