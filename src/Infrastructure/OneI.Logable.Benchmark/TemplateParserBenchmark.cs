namespace OneI.Logable;

using System.Globalization;
using NLog;
using Serilog.Parsing;
using Templatizations;

public class TemplateParserBenchmark
{
    private const string text = "a6sd5f4asdf45as6f{0:yyyy-MM-dd HH:mm:ss}asd6fa4sdf65asdf";

    [Params(100)]
    public int count;

    [Benchmark(Baseline = true)]
    public void UseSeriLog()
    {
        for(var i = 0; i < count; i++)
        {
            var _ = new MessageTemplateParser().Parse(text);
        }
    }

    [Benchmark]
    public void UseLogable()
    {
        for(var i = 0; i < count; i++)
        {
            var _ = TemplateParser.Parse2(text);
        }
    }

    [Benchmark]
    public void UseLogable1()
    {
        for(var i = 0; i < count; i++)
        {
            var _ = TemplateParser.Parse(text.AsMemory());
        }
    }

    [Benchmark]
    public void UseNLog()
    {
        for(var i = 0; i < count; i++)
        {
            var _ = new LogEventInfo(NLog.LogLevel.Debug, "TemplateParserBenchmark", CultureInfo.InvariantCulture,
           text, new object[] { DateTime.Now }).MessageTemplateParameters;
        }
    }
}
