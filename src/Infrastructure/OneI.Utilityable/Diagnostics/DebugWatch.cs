namespace OneI.Diagnostics;

using OneI.Horology;

public static class DebugWatch
{
    private static readonly List<MarkPoint> _points = new();

    private static StopwatchValue? _watch;

    private static MarkPoint? _stop;

    [Conditional(SharedConstants.DEBUG)]
    public static void Mark(
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int line = 0)
    {
        if(_watch.HasValue == false)
        {
            _watch = StopwatchValue.StartNew();
        }

        var current = _watch.Value.GetElapsedTime();

        _points.Add(new(current - (_stop?.timestamp ?? default), file!, member!, line));
    }

    [Conditional(SharedConstants.DEBUG)]
    public static void Stop(
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int line = 0)
    {
        if(_watch.HasValue == false
            || _stop.HasValue == true)
        {
            return;
        }

        _stop = new MarkPoint(_watch.Value.GetElapsedTime(), file!, member!, line);
    }

    [Conditional(SharedConstants.DEBUG)]
    public static void EndAndReport(
        Action<string>? writer = null,
        [CallerFilePath] string? file = null,
        [CallerMemberName] string? member = null,
        [CallerLineNumber] int line = 0)
    {
        Mark(file, member, line);

        _stop = null;
        _watch = null;

        var container = new StringBuilder();

        try
        {
            if(_points.Count == 1)
            {
                var point = _points[0];

                container.AppendLine($"Total：{point.timestamp.TotalMicroseconds:N} ns");

                writer?.Invoke(container.ToString());

                return;
            }

            container.AppendLine();
            TimeSpan totalTimestamp = default;

            for(var i = 0; i < _points.Count - 1; i++)
            {
                var item = _points[i];
                var next = _points[i + 1];

                var diff = next.timestamp - item.timestamp;

                container.Append($"{i + 1}：{TimeInterval.FromNanoseconds(diff.TotalMicroseconds)} (");

                var itemFile = Path.GetFileNameWithoutExtension(item.filepath);
                var nextFile = Path.GetFileNameWithoutExtension(item.filepath);

                container.Append($"{itemFile}#L{item.line}");
                container.Append($" - ");
                container.Append($"{nextFile}#L{next.line}");

                container.AppendLine(")");

                totalTimestamp += diff;
            }

            var total = TimeInterval.FromNanoseconds(totalTimestamp.TotalMicroseconds);
            var avg = total / _points.Count;
            container.Insert(0, $"Total: {total}, Avg: {avg}");

            writer?.Invoke(container.ToString());
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
