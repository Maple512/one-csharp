namespace OneI.Logable;

using OneI.Logable.Middlewares;

public partial class LoggerConfiguration
{
    private class LoggerPropertyConfiguration : ILoggerPropertyConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerPropertyConfiguration(LoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration Add<T>(string name, T value)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);

            return _parent.With(new PropertyMiddleware<T>(name, value));
        }

        public ILoggerConfiguration AddOrUpdate<T>(string name, T value)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);

            return _parent.With(new PropertyMiddleware<T>(name, value, true));
        }
    }
}
