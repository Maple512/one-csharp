namespace OneI.Openable.Connections;

using OneI.Logable;

internal static class ConnectionCompetion
{
    // TODO: 需要优化，不想使用object
    public static Task FireOnCompletedAsync(ILogger logger, Stack<KeyValuePair<Func<object, Task>, object>>? onCompleted)
    {
        if(onCompleted is null or { Count: 0 })
        {
            return Task.CompletedTask;
        }

        return CompleteAsyncMayAwait(logger, onCompleted);
    }

    private static Task CompleteAsyncMayAwait(ILogger logger, Stack<KeyValuePair<Func<object, Task>, object>> onCompleted)
    {
        while(onCompleted.TryPop(out var entry))
        {
            try
            {
                var task = entry.Key.Invoke(entry.Value);

                if(!task.IsCompletedSuccessfully)
                {
                    return CompletedAsyncAwaited(task, logger, onCompleted);
                }
            }
            catch(Exception ex)
            {
                LogError(logger, ex);
            }
        }

        return Task.CompletedTask;
    }

    private static async Task CompletedAsyncAwaited(Task task, ILogger logger, Stack<KeyValuePair<Func<object, Task>, object>> onCompleted)
    {
        try
        {
            await task;
        }
        catch(Exception ex)
        {
            LogError(logger, ex);
        }

        while(onCompleted.TryPop(out var entry))
        {
            try
            {
                await entry.Key.Invoke(entry.Value);
            }
            catch(Exception ex)
            {
                LogError(logger, ex);
            }
        }
    }

    private static void LogError(ILogger logger, Exception ex)
    {
        logger.Error(ex, $"An error occurred running an IConnectionCompleteFeature.OnCompleted callback.");
    }
}
