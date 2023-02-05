namespace OneI.Logable;

using System;
using System.ComponentModel;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using Basic.Reference.Assemblies;
using Cysharp.Text;
using OneI.Logable.Templates;
using OneI.Text;
using OneT.CodeGenerator;
using VerifyXunit;

[UsesVerify]
public class LogGenerator_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task generator_simple()
    {
        var source =
"""
#nullable enable
namespace OneI.Logable.Fakes;

using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using OneI.Logable;

public class UserService
{
    public async Task Register()
    {
        // Log.Error("", "", new object(), new Dictionary<int, int>(), new List<int>());

        var logger = new LoggerConfiguration()
        .CreateLogger();

        logger.Error(new InvalidCastException(),"", Array.Empty<int>(), default(int?), new List<User1>(), new Dictionary<int,int>(), new string[10], 1, (1,2,3,4), new User1());

        //logger.Write(LogLevel.Verbose, new Exception(), "", 1, 1, 1, 1);
        //logger.Error("message", new BitArray(10));
        //logger.Error("message", (dynamic)1);
        //logger.Error("message", true, (byte)1, (sbyte)1, 'c', (decimal)1, (double)1);
        //logger.Error("message", (float)1,(int)1, (uint)1, (nint)1, (nuint)1, (long)1, (ulong)1, (short)1, "", (object)1);

        //logger.Error("message", new List<int>(10));     
        //logger.Error("message", new Dictionary<int,int>(10));
    }
}

#nullable restore
""";
        return Verify(source, new LoggerCodeGenerator());
    }

    [Serializable]
    public struct SturctModel1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SturctModel1Content : IPropertyValueFormattable
    {
        public SturctModel1 Value;

        public void Format(ref Utf16ValueStringBuilder writer, PropertyType type)
        {
            writer.Append('{');

            writer.Append($"{nameof(Value.Id)}:");

            TemplateRenderHelper.WriteSpanFormattable(ref writer, Value.Id, null, null);

            writer.Append('}');
        }
    }

    [Fact]
    public void json_serializer()
    {
        var value = new SturctModel1
        {
            Id = 1,
            Name = "Maple512",
            Description = "Foo",
        };

        JsonSerializer.Serialize(value).ShouldBe(
            """
             {"Id":1,"Name":"Maple512","Description":"Foo"}
             """);
    }
}
