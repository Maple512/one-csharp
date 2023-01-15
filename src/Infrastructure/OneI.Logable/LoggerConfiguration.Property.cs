namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Middlewares;
using OneI.Logable.Templatizations;

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

        public ILoggerConfiguration Add<T>(int index, T value, IPropertyValueFormatter<T?>? formatter = null)
        {
            if(index < 0)
            {
                ThrowHelper.ArgumentOutOfRangeException_Enum_Value();
            }

            return _parent.Use(new PropertyMiddleware<T>(index, value, formatter));
        }

        public ILoggerConfiguration AddOrUpdate<T>(int index, T value, IPropertyValueFormatter<T?>? formatter = null)
        {
            if(index < 0)
            {
                ThrowHelper.ArgumentOutOfRangeException_Enum_Value();
            }

            return _parent.Use(new PropertyMiddleware<T>(index, value, formatter, true));
        }
    }
}
