namespace OneI.Logable;

using Configurations;
using OneI.Logable.Templatizations;

public partial class LoggerConfiguration
{
    private class TemplateConfiguration : ILoggerTemplateConfiguration
    {
        private readonly LoggerConfiguration _parent;

        public TemplateConfiguration(LoggerConfiguration parent)
        {
            _parent = parent;
        }

        public ILoggerConfiguration Default(string template)
        {
            _parent._defaultTemplate = template.AsMemory();

            return _parent;
        }

        public ILoggerConfiguration UseWhen(Func<LoggerMessageContext, bool> condition, scoped ReadOnlySpan<char> template)
        {
            _parent._templateTokens.Add(new TemplateProvider(condition, template));

            return _parent;
        }
    }
}
