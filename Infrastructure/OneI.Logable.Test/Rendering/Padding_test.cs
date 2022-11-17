namespace OneI.Logable.Rendering;

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public class Padding_test
{
    [Fact]
    public void alignment_right()
    {
        var output = new FileWriter(Path.Combine(TestHelpler.LogFolder, $"{nameof(Padding_test)}.txt"));

        Padding.Apply(output, "asd3f54ads65f", new(Direction.Left, 100));

        output.WriteLine();

        Padding.Apply(output, "asd3f54ads65f", new(Direction.Right, 100));

        Debug.WriteLine(output.ToString());
    }
}

public class FileWriter : TextWriter
{
    private readonly string _file;

    public FileWriter(string file)
    {
        _file = file;

        var directory = Path.GetDirectoryName(file)!;

        if(Directory.Exists(directory) == false)
        {
            Directory.CreateDirectory(directory);
        }

        if(File.Exists(_file))
        {
            File.Delete(_file);
        }

        File.Create(_file).Close();
    }

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(string? value)
    {
        File.AppendAllText(_file, value);
    }

    public override void WriteLine()
    {
        File.AppendAllText(_file, Environment.NewLine);
    }
}
