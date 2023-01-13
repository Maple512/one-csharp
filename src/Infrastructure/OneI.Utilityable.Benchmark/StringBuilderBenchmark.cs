namespace OneI.Utilityable;

using System.Buffers;
using DotNext.Buffers;
using OneI.Text;

public class StringBuilderBenchmark
{
    private const string StringValue = "1234567890abcdefghijklmnopqrstuvwxyz1234567890abcdefghijklmnopqrstuvwxyz";

    public const int count = 50;
    public const int capacity = GlobalConstants.StringFormatMinimumLength;

    public void Validate()
    {
        var result = UseStringBuilder();

        IValidator.AreEquals(UseList(), result, StringComparer.OrdinalIgnoreCase);
        IValidator.AreEquals(UseValueStringBuilder(), result, StringComparer.OrdinalIgnoreCase);
        IValidator.AreEquals(UseRefValueStringBuilder(), result, StringComparer.OrdinalIgnoreCase);
        IValidator.AreEquals(UsePooledArrayBufferWriter(), result, StringComparer.OrdinalIgnoreCase);
        IValidator.AreEquals(UseSparseBufferWriter(), result, StringComparer.OrdinalIgnoreCase);
        IValidator.AreEquals(UseBufferWriterSlim(), result, StringComparer.OrdinalIgnoreCase);
    }

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
    public string UsePooledArrayBufferWriter()
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
    public string UseSparseBufferWriter()
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
    public string UseBufferWriterSlim()
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
}
