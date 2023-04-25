namespace OneI.Openable.Connections.Sockets.Internal;

using OneI.Logable;

internal static class LoggerSocketExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConnectionReset(this ILogger logger, string connectionId)
    {
        logger.Debug($"Connection id \"{connectionId}\" reset.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConnectionError(this ILogger logger, string connectionId, Exception ex)
    {
        logger.Debug(ex, $"Connection id \"{connectionId}\" communication error.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConnectionPaused(this ILogger logger, string connectionId)
    {
        logger.Debug($"Connection id \"{connectionId}\" paused.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ConnectionWriteFIN(this ILogger logger, string connectionId, Exception ex)
    {
        logger.Debug(ex, $"Connection id \"{connectionId}\" sending FIN.");
    }
}
