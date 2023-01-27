namespace OneI.Diagnostics;

using Perfolizer.Horology;

public static class DebugWatcher
{
    /// <summary>
    /// 调试报告接收器
    /// </summary>
    /// <param name="mssage">消息文本</param>
    /// <param name="totalSeconds">总用时</param>
    /// <param name="avgSeconds">平均用时</param>
    public delegate void DebugReportReciver(string mssage, double totalSeconds, double avgSeconds);

    private static readonly List<MarkPoint> _points = new();

    private static StartedClock? clock;
    private static readonly object _lock = new();

    private static TimeSpan? _stop;

    [Conditional(SharedConstants.DEBUG)]
    public static void Mark(
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int line = 0)
    {
        lock(_lock)
        {
            if(clock.HasValue == false)
            {
                clock = Chronometer.Start();
            }

            var current = clock.Value.GetElapsed().GetTimeSpan() - _stop.GetValueOrDefault();

            _points.Add(new(current, file!, member!, line));
        }
    }

    [Conditional(SharedConstants.DEBUG)]
    public static void Stop(
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int line = 0)
    {
        if(clock == null
            || _stop.HasValue)
        {
            return;
        }

        lock(_lock)
        {
            _stop = clock.Value.GetElapsed().GetTimeSpan();
        }
    }

    /// <summary>
    /// 结束调试，并将结果输出到给定的接收器中
    /// </summary>
    /// <param name="receiver">结果接收器</param>
    [Conditional(SharedConstants.DEBUG)]
    public static void EndAndReport(
        DebugReportReciver? receiver = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int line = 0)
    {
        Mark(file, member, line);

        _stop = null;
        clock = null;

        var container = new StringBuilder();

        try
        {
            if(_points.Count == 1)
            {
                var point = _points[0];
                var interval = TimeInterval.FromNanoseconds(point.timestamp.TotalNanoseconds);
                container.AppendLine($"Total：{interval} ns");

                var totalSeconds = interval.ToSeconds();
                receiver?.Invoke(container.ToString(), totalSeconds, totalSeconds);

                return;
            }

            container.AppendLine();
            var totalTimestamp = TimeSpan.Zero;

            for(var i = 0; i < _points.Count - 1; i++)
            {
                var item = _points[i];
                var next = _points[i + 1];

                var diff = next.timestamp - item.timestamp;

                container.Append($"{i + 1}：{TimeInterval.FromNanoseconds(diff.TotalNanoseconds)} (");

                var itemFile = Path.GetFileNameWithoutExtension(item.filepath);
                var nextFile = Path.GetFileNameWithoutExtension(item.filepath);

                container.Append($"{itemFile}#L{item.line}");
                container.Append(" - ");
                container.Append($"{nextFile}#L{next.line}");

                container.AppendLine(")");

                totalTimestamp += diff;
            }

            var total = TimeInterval.FromNanoseconds(totalTimestamp.TotalNanoseconds);
            var avg = total / _points.Count;
            container.Insert(0, $"Total: {total}, Avg: {avg}");

            receiver?.Invoke(container.ToString(), total.ToSeconds(), avg.ToSeconds());
        }
        finally
        {
            _points.Clear();
        }
    }

    private readonly struct MarkPoint
    {
        public readonly TimeSpan timestamp;
        public readonly string filepath;
        public readonly string member;
        public readonly int line;

        public MarkPoint(TimeSpan timestamp, string filepath, string member, int line)
        {
            this.timestamp = timestamp;
            this.filepath = filepath;
            this.member = member;
            this.line = line;
        }
    }
}
