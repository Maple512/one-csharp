namespace OneI.Logable.Endpoints;

/// <summary>
/// The aggregate endpoint.
/// </summary>
public class AggregateEndpoint : ILoggerEndpoint
{
    private readonly IReadOnlyList<ILoggerEndpoint> _endpoints;
    private readonly bool _isSilent;

    /// <summary>
    /// 聚合多个<see cref="ILoggerEndpoint"/>
    /// </summary>
    /// <param name="endpoints"></param>
    /// <param name="isSilent">是否忽略遇到的异常</param>
    public AggregateEndpoint(List<ILoggerEndpoint> endpoints, bool isSilent = false)
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
                    (exceptions ?? new()).Add(ex);
                }
            }
        }

        if(_isSilent == false && exceptions != null)
        {
            throw new AggregateException(exceptions);
        }
    }
}
