namespace OneI.Textable;

public class TextTemplateBenchmark
{
    const string format = "{0},{1},{2},{3},{4},{5},{0},{1}";

    static int arg1 = 1;
    static string arg2 = "arg2";
    static bool arg3 = false;
    static char arg4 = 'a';
    static Model1 arg5 = new Model1(1, "Maple512", "随遇而安，积极向上");
    static Exception arg6 = new InvalidOperationException("invalid operation exception");

    [Benchmark(Baseline = true)]
    public string UseStringFormat()
    {
        return string.Format(format, arg1, arg2, arg3, arg4, arg5, arg6);
    }

    [Benchmark]
    public string UseValueStringBuilder()
    {
        var builder = new ValueStringBuilder(GlobalConstants.ArrayPoolMinimumLength);

        builder.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6);

        return builder.ToString();
    }

    [Benchmark]
    public string UseStringBuilder()
    {
        var builder = new StringBuilder(GlobalConstants.ArrayPoolMinimumLength);

        builder.AppendFormat(format, arg1, arg2, arg3, arg4, arg5, arg6);

        return builder.ToString();
    }

    [Benchmark]
    public string UseTextTemplate()
    {
        using var template = TextTemplate.Create(format)
              .AddProperty(arg1)
              .AddProperty(arg2)
              .AddProperty(arg3)
              .AddProperty(arg4)
              .AddProperty(arg5)
              .AddProperty(arg6);

        return template.ToString();
    }

    class Model1
    {
        public Model1(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public int Id { get; }

        public string Name { get; }

        public string Description { get; }
    }
}
