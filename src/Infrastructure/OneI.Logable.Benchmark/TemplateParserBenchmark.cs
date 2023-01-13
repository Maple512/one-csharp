namespace OneI.Logable;

using System.Globalization;
using OneI.Logable.Templatizations;
using Serilog.Parsing;

public class TemplateParserBenchmark
{
    private const string text = "a6sd5f4asdf45as6f{0:yyyy-MM-dd HH:mm:ss}asd6fa4sdf65asdf";

    [Benchmark(Baseline = true)]
    public void UseSeriLog()
    {
        var _ = new MessageTemplateParser().Parse(text);
    }

    [Benchmark]
    public void UseLogable()
    {
        var _ = TemplateParser.Parse(text);
    }

    [Benchmark]
    public void UseNLog()
    {
        var _ = new NLog.LogEventInfo(NLog.LogLevel.Debug, "TemplateParserBenchmark", CultureInfo.InvariantCulture,
            text, new object[] { DateTime.Now }).MessageTemplateParameters;
    }

    [Benchmark]
    public void UseStringBuilder()
    {
        var _ = new StringBuilder().AppendFormat(text, DateTime.Now);
    }
}
