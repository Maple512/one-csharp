namespace OneI.Logable.Sinks;

public class AggregateSink : ILoggerSink
{
    private readonly IReadOnlyList<ILoggerSink> _endpoints;
    private readonly bool _isSilent;

    public AggregateSink(List<ILoggerSink> endpoints, bool isSilent = false)
    {
        _endpoints = endpoints;
        _isSilent = isSilent;
    }

    public void Invoke(in LoggerContext context)
    {
        List<Exception>? exceptions = null;

        foreach(var endpoint in _endpoints)
        {
            try
            {
                endpoint.Invoke(context);
            }
            catch(Exception ex)
            {
                InternalLog.WriteLine($"Exception received when Invoke is called within {endpoint}: {ex}");

                if(_isSilent == false)
                {
                    (exceptions ?? new List<Exception>()).Add(ex);
                }
            }
        }

        if(_isSilent == false && exceptions != null)
        {
            throw new AggregateException(exceptions);
        }
    }
}
