namespace OneI.Logable;

using Configurations;
using Middlewares;
using Templatizations;

public partial class LoggerConfiguration
{
    private class LoggerPropertyConfiguration : ILoggerPropertyConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public LoggerPropertyConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration Add<T>(string name, T value, IPropertyValueFormatter<T?>? formatter = null)
        {
            Check.NotNullOrEmpty(name);

            return _parent.Use(new PropertyMiddleware<T>(name, value, formatter));
        }

        public ILoggerConfiguration AddOrUpdate<T>(string name, T value, IPropertyValueFormatter<T?>? formatter = null)
        {
            Check.NotNullOrEmpty(name);

            return _parent.Use(new PropertyMiddleware<T>(name, value, formatter, true));
        }
    }
}
