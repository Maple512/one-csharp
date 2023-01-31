namespace OneI.Utilityable;

using System.Globalization;
using BenchmarkDotNet.Columns;
using Cysharp.Text;

[GroupBenchmarksBy]
public class StringTransferBenchmark : BenchmarkItem
{
    private static readonly string appended = Randomizer.String(20);
    private static readonly string text = Randomizer.String(100);

    [Params(100)]
    public int length;
    public override void GlobalInlitialize()
    {
        length = 100;
        var result = UseString();

        var bytes = Encoding.UTF8.GetByteCount(string.Concat(result));

        var descriptor = SizeValue.FromBytes(bytes).ToString(SizeUnit.KB, CultureInfo.InvariantCulture);

        Console.WriteLine($"***********************Original length: {descriptor}**************************");

        SequenceEquals(result, UseMemory());
        SequenceEquals(result, UseZString());
    }

    // 2
    [Benchmark(Baseline = true)]
    public List<string> UseString()
    {
        var result = new List<string>();

        for (var i = 0; i < length; i++)
        {
            result.Add(StringTransfer(text));
        }

        return result;
    }

    // 3
    [Benchmark]
    public List<string> UseMemory()
    {
        var result = new List<string>();

        for (var i = 0; i < length; i++)
        {
            var t = text.AsMemory();

            MemoryTransfer0(ref t);

            result.Add(t.ToString());
        }

        return result;
    }

    // 1
    [Benchmark]
    public List<string> UseZString()
    {
        var result = new List<string>();

        for (var i = 0; i < length; i++)
        {
            var builder = new Utf16ValueStringBuilder(true);

            builder.Append(text);

            ZStringTransfer0(ref builder);

            result.Add(builder.ToString());

            builder.Dispose();
        }

        return result;
    }

    private static string StringTransfer(string input)
    {
        input = StringTransfer1(input);
        input = StringTransfer2(input);
        input = StringTransfer3(input);
        input = StringTransfer4(input);

        return input;
    }

    private static string StringTransfer1(string input) => string.Concat(input, appended);
    private static string StringTransfer2(string input) => string.Concat(input, appended);
    private static string StringTransfer3(string input) => string.Concat(input, appended);
    private static string StringTransfer4(string input) => string.Concat(input, appended);

    private static void MemoryTransfer0(ref ReadOnlyMemory<char> input)
    {
        MemoryTransfer1(ref input);
        MemoryTransfer2(ref input);
        MemoryTransfer3(ref input);
        MemoryTransfer4(ref input);
    }
    private static void MemoryTransfer1(ref ReadOnlyMemory<char> input) { input = string.Concat(input.Span, appended).AsMemory(); }
    private static void MemoryTransfer2(ref ReadOnlyMemory<char> input) { input = string.Concat(input.Span, appended).AsMemory(); }
    private static void MemoryTransfer3(ref ReadOnlyMemory<char> input) { input = string.Concat(input.Span, appended).AsMemory(); }
    private static void MemoryTransfer4(ref ReadOnlyMemory<char> input) { input = string.Concat(input.Span, appended).AsMemory(); }

    private static void ZStringTransfer0(ref Utf16ValueStringBuilder input)
    {
        ZStringTransfer1(ref input);
        ZStringTransfer2(ref input);
        ZStringTransfer3(ref input);
        ZStringTransfer4(ref input);
    }
    private static void ZStringTransfer1(ref Utf16ValueStringBuilder input)
    {
        input.Append(appended);
    }
    private static void ZStringTransfer2(ref Utf16ValueStringBuilder input)
    {
        input.Append(appended);
    }
    private static void ZStringTransfer3(ref Utf16ValueStringBuilder input)
    {
        input.Append(appended);
    }
    private static void ZStringTransfer4(ref Utf16ValueStringBuilder input)
    {
        input.Append(appended);
    }
}
