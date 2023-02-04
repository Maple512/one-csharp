namespace OneI.Utilityable.Strings;

using System.Buffers;
using Cysharp.Text;
using DotNext.Buffers;
using OneI.Text;

public class StringBuilderBenchmark
{
    public const int count = 50;

    [Params(50, 10000)]
    public int stringCount;

    [Params(50, 10000)]
    public int capacity;

    // 2
    [Benchmark(Baseline = true)]
    public string UseStringBuilder()
    {
        var StringValue = Randomizer.String(stringCount);

        var writer = new StringBuilder(capacity);
        try
        {
            for(var i = 0; i < count; i++)
            {
                writer.Append(StringValue);

                writer.AppendLine();
            }

            return writer.ToString();
        }
        finally
        {
            writer.Clear();
        }
    }

    //4
    [Benchmark]
    public string UseList()
    {
        var StringValue = Randomizer.String(stringCount);
        var builder = new List<string>();

        for(var i = 0; i < count; i++)
        {
            builder.Add(StringValue);
            builder.Add(Environment.NewLine);
        }

        return string.Concat(builder);
    }

    // 2
    [Benchmark]
    public string UseValueStringBuilder()
    {
        var StringValue = Randomizer.String(stringCount);

        using var builder = new Utf16ValueStringBuilder(true);

        for(var i = 0; i < count; i++)
        {
            builder.Append(StringValue);
            builder.AppendLine();
        }

        return builder.ToString();
    }
    // 1
    [Benchmark]
    public string UseRefValueStringBuilder()
    {
        var StringValue = Randomizer.String(stringCount);
        using scoped var builder = new RefValueStringBuilder(capacity);

        for(var i = 0; i < count; i++)
        {
            builder.Append(StringValue);
            builder.AppendLine();
        }

        return builder.ToString();
    }

    //2
    [Benchmark]
    public string UsePooledArrayBufferWriter()
    {
        var StringValue = Randomizer.String(stringCount);
        using var writer = new PooledArrayBufferWriter<char>
        {
            Capacity = capacity
        };

        for(var i = 0; i < count; i++)
        {
            writer.Write(StringValue);
            writer.WriteLine();
        }

        return writer.ToString();
    }

    // 5
    [Benchmark]
    public string UseSparseBufferWriter()
    {
        var StringValue = Randomizer.String(stringCount);
        using var writer = new SparseBufferWriter<char>(capacity);
        for(var i = 0; i < count; i++)
        {
            writer.Write(StringValue);
            writer.WriteLine();
        }

        return writer.ToString();
    }
    //3
    [Benchmark]
    public string UseBufferWriterSlim()
    {
        var StringValue = Randomizer.String(stringCount);
        var writer = new BufferWriterSlim<char>(capacity);
        try
        {
            for(var i = 0; i < count; i++)
            {
                writer.Write(StringValue);
                writer.WriteLine();
            }

            return writer.ToString();
        }
        finally
        {
            writer.Dispose();
        }
    }
}
