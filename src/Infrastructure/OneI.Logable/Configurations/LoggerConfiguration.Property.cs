namespace OneI.Logable;

using OneI.Logable.Middlewares;

public partial class LoggerConfiguration
{
    private class LoggerPropertyConfiguration : ILoggerPropertyConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerPropertyConfiguration(LoggerConfiguration parent) => _parent = parent;

        public ILoggerConfiguration Add(string name, object value)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);

            return _parent.With(new PropertyMiddleware(name, value));
        }

        public ILoggerConfiguration AddOrUpdate(string name, object value)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);

            return _parent.With(new PropertyMiddleware(name, value, true));
        }
    }
}
