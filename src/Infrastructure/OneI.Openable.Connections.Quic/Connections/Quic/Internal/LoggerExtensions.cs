namespace OneI.Openable.Connections.Quic.Internal;

using OneI.Logable;

internal static class LoggerExtensions
{
    public static void AcceptedConnection(this ILogger logger, string connectionId)
    {
        logger.Debug($"Connection id \"{connectionId}\" accepted.");
    }

    public static void AcceptedStream(this ILogger logger, QuicStreamContext context)
    {
        logger.Debug($"Stream id \"{context.ConnectionId}\" type {GetStreamType(context)} accepted.");
    }

    public static void ConnectedStream(this ILogger logger, QuicStreamContext context)
    {
        logger.Debug($"Stream id \"{context.ConnectionId}\" type {GetStreamType(context)} connected.");
    }

    public static void ConnectionError(this ILogger logger, string connectionId, Exception ex)
    {
        logger.Debug(ex, $"Connection id \"{connectionId}\" unexpected error.");
    }

    public static void ConnectionAborted(this ILogger logger, string connectionId, long errorCode, Exception ex)
    {
        logger.Debug(ex, $"Connection id \"{connectionId}\" aborted by peer with error code {errorCode}.");
    }

    public static void ConnectionAbort(this ILogger logger, string connectionId, long errorCode, Exception ex)
    {
        logger.Debug(ex, $"Connection id \"{connectionId}\" aborted by application with error code {errorCode}.");
    }

    public static void StreamError(this ILogger logger, string connectionId, Exception ex)
    {
        logger.Debug(ex, $"Stream id \"{connectionId}\" unexpected error.");
    }

    public static void StreamPause(this ILogger logger, string connectionId)
    {
        logger.Debug($"Stream id \"{connectionId}\" paused.");
    }

    public static void StreamResume(this ILogger logger, string connectionId)
    {
        logger.Debug($"Stream id \"{connectionId}\" resumed.");
    }

    public static void StreamReused(this ILogger logger, string connectionId)
    {
        logger.Debug($"Stream id \"{connectionId}\" resumed from pool.");
    }

    public static void StreamTimeoutRead(this ILogger logger, string connectionId)
    {
        logger.Debug($"Stream id \"{connectionId}\" read timed out.");
    }

    public static void StreamTimeoutWrite(this ILogger logger, string connectionId)
    {
        logger.Debug($"Stream id \"{connectionId}\" write timed out.");
    }

    public static void StreamAbort(this ILogger logger, string connectionId, long errorCode, Exception ex)
    {
        logger.Debug(ex, $"Stream id \"{connectionId}\" aborted by application with error code {errorCode}.");
    }

    public static void StreamAbortRead(this ILogger logger, string connectionId, long errorCode, Exception ex)
    {
        logger.Debug(ex, $"Stream id \"{connectionId}\" read side aborted by application with error code {errorCode}.");
    }

    public static void StreamAbortWrite(this ILogger logger, string connectionId, long errorCode, Exception ex)
    {
        logger.Debug(ex, $"Stream id \"{connectionId}\" write side aborted by application with error code {errorCode}.");
    }

    public static void StreamPooledReuse(this ILogger logger, string connectionId)
    {
        logger.Debug($"Stream id \"{connectionId}\" pooled for reuse.");
    }

    public static void StreamShutdownWrite(this ILogger logger, string connectionId, Exception ex)
    {
        logger.Debug(ex, $"Stream id \"{connectionId}\" shutting down writes.");
    }

    public static void StreamAbortedRead(this ILogger logger, string connectionId, long errorCode)
    {
        logger.Debug($"Stream id \"{connectionId}\" read aborted by peer with error code {errorCode}.");
    }

    public static void StreamAbortedWrite(this ILogger logger, string connectionId, long errorCode)
    {
        logger.Debug($"Stream id \"{connectionId}\" write aborted by peer with error code {errorCode}.");
    }

    private static StreamType GetStreamType(QuicStreamContext context)
        => context.CanRead && context.CanWrite
        ? StreamType.Bidirectional
        : StreamType.Unidirectional;

    private enum StreamType
    {
        Unidirectional,
        Bidirectional
    }
}
