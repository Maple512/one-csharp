namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;

public partial class LoggerConfiguration
{
    private class LoggerPropertyConfiguration : ILoggerPropertyConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerPropertyConfiguration(LoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration Add<T>(string name, T value)
        {
            _ = Check.NotNullOrEmpty(name);

            return _parent.Use(new PropertyMiddleware<T>(name, value));
        }

        public ILoggerConfiguration AddOrUpdate<T>(string name, T value)
        {
            _ = Check.NotNullOrEmpty(name);

            return _parent.Use(new PropertyMiddleware<T>(name, value, true));
        }
    }
}
