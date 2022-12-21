namespace OneI.Textable.Fakes;

using System;
using System.IO;

internal class Model1
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }

}

internal class ModelFormatter : IFormatter<Model1>
{
    public void Format(Model1? value, TextWriter writer, string? format = null, IFormatProvider? formatProvider = null)
    {
        writer.Write("Custom formatter");
    }
}
