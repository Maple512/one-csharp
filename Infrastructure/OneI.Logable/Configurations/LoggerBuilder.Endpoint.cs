namespace OneI.Logable;

using OneI.Logable.Configurations;
using OneI.Logable.Endpoints;

public partial class LoggerBuilder
{
    public class LoggerEndpointBuilder : ILoggerEndpointBuilder
    {
        private readonly LoggerBuilder _parent;

        public LoggerEndpointBuilder(LoggerBuilder parent)
        {
            _parent = parent;
        }

        public ILoggerBuilder RunWhen(Func<LoggerContext, bool> condition, ILoggerEndpoint endpoint)
        {
            _parent._endpoints.Add(new ConditionalEndpoint(condition, endpoint));

            return _parent;
        }

        public ILoggerBuilder Run(ILoggerEndpoint endpoint)
        {
            _parent._endpoints.Add(endpoint);

            return _parent;
        }
    }
}
