namespace System.IO;

using System;
using System.Text;
using System.Text.Json;
using OneT.Common;
using Xunit;

public class TextWriter_Test
{
    [Fact]
    public void writer_flush()
    {
        Nullable<Model> a = new Model(1);

        var aa = JsonSerializer.Serialize(a);

        var filePath = TestTools.GetFilePathWithinProject("./Logs/aaa.txt");

        var ms = System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

        var stream = new StreamWriter(ms, Encoding.UTF8);

        stream.Write($"{DateTimeOffset.Now:O}");

        stream.Flush();

        ms.Flush(true);

        stream.Dispose();

        ms.Dispose();
    }

    private readonly struct Model
    {
        public Model(int id) => Id = id;

        public int Id { get; }
    }
}
