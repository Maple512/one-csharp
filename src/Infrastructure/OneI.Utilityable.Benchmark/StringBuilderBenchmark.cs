namespace OneI.Utilityable;

using System.Buffers;
using DotNext.Buffers;

public class StringBuilderBenchmark
{
    private const string StringValue = "1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz";

    [Params(5, 50)]
    public int count;

    [Params(GlobalConstants.ArrayPoolMinimumLength)]
    public int capacity;

    [Benchmark(Baseline = true)]
    public string UseStringBuilder()
    {
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

    [Benchmark]
    public string UseList()
    {
        var builder = new List<string>();

        for(var i = 0; i < count; i++)
        {
            builder.Add(StringValue);
            builder.Add(Environment.NewLine);
        }

        return string.Join(null, builder);
    }

    [Benchmark]
    public string UseValueStringBuilder()
    {
        var builder = new ValueStringBuilder(capacity);

        for(var i = 0; i < count; i++)
        {
            builder.Append(StringValue);
            builder.AppendLine();
        }

        return builder.ToString();
    }

    [Benchmark]
    public string UseRefValueStringBuilder()
    {
        using scoped var builder = new RefValueStringBuilder(capacity);

        for(var i = 0; i < count; i++)
        {
            builder.Append(StringValue);
            builder.AppendLine();
        }

        return builder.ToString();
    }

    [Benchmark]
    public string UseValueStringBuffer()
    {
        var handler = new ValueBuffer(capacity);

        for(var i = 0; i < count; i++)
        {
            handler.Append(StringValue);
            handler.AppendLine();
        }

        return handler.ToStringAndClear();
    }

    [Benchmark]
    public string BuildStringUsingPooledArrayBufferWriter()
    {
        using var writer = new PooledArrayBufferWriter<char>()
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

    [Benchmark]
    public string BuildStringUsingSparseBufferWriter()
    {
        using var writer = new SparseBufferWriter<char>(capacity);
        for(var i = 0; i < count; i++)
        {
            writer.Write(StringValue);
            writer.WriteLine();
        }

        return writer.ToString();
    }

    [Benchmark]
    public string BuildStringOnStackNoPreallocatedBuffer()
    {
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

    [Benchmark]
    public string BuildStringOnStack()
    {
        var writer = new BufferWriterSlim<char>(stackalloc char[capacity]);
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
