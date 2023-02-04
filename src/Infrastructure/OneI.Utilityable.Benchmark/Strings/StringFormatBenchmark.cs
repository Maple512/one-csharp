namespace OneI.Utilityable.Strings;

using Cysharp.Text;

public class StringFormatBenchmark : BenchmarkItem
{
    private const string format = "{0},{1}";
    private static User user = new(512, "Maple512");
    private const int capacity = 256;
    private const int count = 100;

    public override void Inlitialize()
    {
        var result = UseStringBuilder();

        AreEquals(result, UseList());

        AreEquals(result, UseZString());
    }

    [Benchmark(Baseline = true)]
    public string UseStringBuilder()
    {
        var writer = new StringBuilder(capacity);
        try
        {
            for(var i = 0; i < count; i++)
            {
                _ = writer.AppendFormat(format, 100, user);
                _ = writer.AppendLine();
            }

            return writer.ToString();
        }
        finally
        {
            _ = writer.Clear();
        }
    }

    [Benchmark]
    public string UseList()
    {
        var builder = new List<string>(capacity);

        for(var i = 0; i < count; i++)
        {
            builder.Add(string.Format(format, 100, user));
            builder.Add(Environment.NewLine);
        }

        return string.Join(null, builder);
    }

    [Benchmark]
    public string UseZString()
    {
        using var writer = new Utf16ValueStringBuilder(true);

        for(var i = 0; i < count; i++)
        {
            writer.AppendFormat(format, 100, user);
            writer.AppendLine();
        }

        return writer.ToString();
    }

    private struct User
    {
        public int Id;
        public string Name;

        public User(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
