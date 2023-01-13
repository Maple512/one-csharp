namespace OneI.Logable;

using System;
using OneI.Logable.Configurations;

public partial class LoggerConfiguration
{
    private class TemplateConfiguration : ILoggerTemplateConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public TemplateConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, string template)
        {
            _parent._templateTokens.Add(context =>
            {
                if(condition(context))
                {
                    return template;
                }

                return null;
            });

            return _parent;
        }
    }
}
