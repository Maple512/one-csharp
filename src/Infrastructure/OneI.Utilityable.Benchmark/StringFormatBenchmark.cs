namespace OneI.Utilityable;

using OneI.Text;

public class StringFormatBenchmark
{
    private const string format = "{0},{1}";
    private static User user = new User(512, "Maple512");
    private const int capacity = 256;
    private const int count = 100;

#if DEBUG
    public void Validate()
    {
        var result = UseStringBuilder();

        Shouldly.ShouldBeTestExtensions.ShouldBe(result, UseList());

        var a = UseValueStringBuilder();

        Shouldly.ShouldBeTestExtensions.ShouldBe(result, a);
    }
#endif

    [Benchmark(Baseline = true)]
    public string UseStringBuilder()
    {
        var writer = new StringBuilder(capacity);
        try
        {
            for(var i = 0; i < count; i++)
            {
                writer.AppendFormat(format, 100, user);
                writer.AppendLine();
            }

            return writer.ToString();
        }
        finally
        {
            writer.Clear();
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
    public string UseValueStringBuilder()
    {
        var writer = new ValueStringBuilder(capacity);

        for(var i = 0; i < count; i++)
        {
            writer.AppendFormat(null, format, 100, user);
            writer.AppendLine();
        }

        return writer.ToString();
    }

    struct User
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
