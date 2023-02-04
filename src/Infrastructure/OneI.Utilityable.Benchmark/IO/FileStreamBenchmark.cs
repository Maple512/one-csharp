namespace OneI.Utilityable.IO;

using DotNext.IO;
using Microsoft.Extensions.Logging;
using ZLogger;
using ZLogger.Entries;

public class FileStreamBenchmark : BenchmarkItem
{
    [Params(100)]
    public int length;

    public FileStreamOptions Options = null!;
    public string Directory = "./Temp";
    private const int bufferSize = 65536;
    private const int timesStringLength = 1024;

    public override void Inlitialize()
    {
        Options = new FileStreamOptions
        {
            Access = FileAccess.Write,
            Share = FileShare.Read,
            BufferSize = bufferSize,
            Mode = FileMode.Create,
            Options = FileOptions.None
        };

        IOTools.EnsureDirectory(Directory);

#if DEBUG
        UseFileStream();

        UseDotNext();

        UseZLogger();
#endif
    }

    [Benchmark(Baseline = true)]
    public void UseFileStream()
    {
        var file = new FileInfo(Path.Combine(Directory, $"{nameof(UseFileStream)}_{Path.GetRandomFileName()}"));

        using var fs = new StreamWriter(File.Open(file.FullName, Options), Encoding.UTF8);

        for(var i = 0; i < length; i++)
        {
            fs.Write(Randomizer.String(timesStringLength));
        }
    }

    [Benchmark]
    public void UseZLogger()
    {
        var file = Path.Combine(Directory, $"{nameof(UseZLogger)}_{Path.GetRandomFileName()}");

        var stream = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.ReadWrite, 1, false);

        var writer = new AsyncStreamLineMessageWriter(stream, new ZLoggerOptions());

        for(var i = 0; i < length; i++)
        {
            var entry = new MessageLogState<object>(null, Randomizer.String(timesStringLength))
                .CreateLogEntry(new LogInfo(nameof(UseZLogger), DateTimeOffset.Now, LogLevel.Information, 0, null));

            writer.Post(entry);
        }

        writer.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    [Benchmark]
    public void UseDotNext()
    {
        var file = Path.Combine(Directory, $"{nameof(UseDotNext)}_{Path.GetRandomFileName()}");

        using var writer = new FileBufferingWriter(new FileBufferingWriter.Options
        {
            AsyncIO = false,
            FileBufferSize = 0,
            FileName = file,
        }).AsTextWriter(Encoding.UTF8);

        for(var i = 0; i < length; i++)
        {
            writer.Write(Randomizer.String(timesStringLength));
        }
    }
}
