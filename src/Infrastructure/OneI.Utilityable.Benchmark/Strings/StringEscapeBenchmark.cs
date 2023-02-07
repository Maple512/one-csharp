namespace OneI.Utilityable.Strings;

using System.Text.Encodings.Web;
using System.Text.Json;
using BenchmarkDotNet.Columns;
using Cysharp.Text;

public class StringEscapeBenchmark : BenchmarkItem
{
    private const string chars =
            """""""""""
            🐛🔥📝
            Mumbai, India|13,922,125	
             Shanghai, China	13,831,900
             Karachi, Pakistan|12,991,000
             Delhi, India	12,259,230
             Istanbul, Turkey|11,372,613
             "asdfasdf""sdf"
            """"""""""";

    [Params(100)]
    public int length;

    public override void Inlitialize()
    {
        Console.WriteLine($"********************************{SizeValue.FromBytes(chars.Length * 2 * length).ToString(null)}********************************");

        UseUri();
        UseJsonSerializer();
        UseJsonHelpers();
        UseJavaScriptEncoder();
    }

    // %F0%9F%90%9B%F0%9F%94%A5%F0%9F%93%9D%0D%0AMumbai%2C%20...
    [Benchmark(Baseline = true)]
    public void UseUri()
    {
        _ = Uri.EscapeDataString(chars);
    }

    // "\uD83D\uDC1B\uD83D\uDD25\uD83D\uDCDD\r\nMumbai,...
    [Benchmark]
    public void UseJsonSerializer()
    {
        _ = JsonSerializer.Serialize(chars);
    }

    // \uD83D\uDC1B\uD83D\uDD25\uD83D\uDCDD\r\nMumbai, 
    [Benchmark]
    public void UseJsonHelpers()
    {
        scoped Span<byte> bytes = new byte[chars.Length * 2];

        _ = Encoding.UTF8.GetBytes(chars, bytes);
        _ = JsonHelpers.Escape(bytes);
    }

    // \uD83D\uDC1B\uD83D\uDD25\uD83D\uDCDD\r\nMumbai, ...
    [Benchmark]
    public void UseJavaScriptEncoder()
    {
        using var source = ZString.CreateUtf8StringBuilder(true);

        scoped var destination = source.GetSpan(chars.Length * 2);

        _ = JavaScriptEncoder.Default.EncodeUtf8(
            Encoding.UTF8.GetBytes(chars),
            destination,
            out var bytesConsumed,
            out var bytesWriitten);

        source.Advance(bytesWriitten);

        var result = source.ToString();
    }
}
