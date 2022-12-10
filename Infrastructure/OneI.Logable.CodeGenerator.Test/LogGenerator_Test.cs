namespace OneI.Logable;

using System.Text.Json;
using System.Threading.Tasks;
using OneI.Logable.Templating;
using OneI.Logable.Templating.Properties.ValueTypes;
using OneT.CodeGenerator;

[UsesVerify]
public class LogGenerator_Test : GeneratorSnapshotTest
{
    [Fact]
    public Task generator_simple()
    {
        // The source code to test
        var source = """
#pragma warning disable IDE0005
#nullable enable
namespace Tests;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OneI.Logable;

public class UserController
{
    private void Index<T0, T1>(T0 t0, T1 t1)
        where T0 : new()
    {

Log.Write(LogLevel.Information, "Message", 1);
        Log.Write(LogLevel.Debug, "Message", 3, 4, 5, 65);

        object o = 1;
        int? a = null;
        var e = new ArgumentException("");
        var u = new User<int>();
        var list = new List<User<int>>();
        var dic = new Dictionary<int, int>();
        var func = static (int a) => "";

        var tulp = (1, 2);

        var array = Array.Empty<string>();

        dynamic dy = 1;

        Log.Write(
            LogLevel.Information,
            "message",
            t0, t1,

            e,
LocalA()
u,
            list,
            async () => await Task.CompletedTask,
            LogGenerator_Test.PublicMethod111,
            RVoid,
            func,
            func(1),
            ()=>string.Empty,
            (int i)=>string.Empty,
            async () => await Task.FromCanceled,
            async () => await Task.FromResult(1),
            "message",
            1,
            (int?)1,
            (byte?)1,
            o,
            tulp,
            a,
            dic,
            array,
            dy,
            LocalM()
            );
    }

    public static int RVoid() { return 1; }

    static string LocalM() => string.Empty;
    static IEnumerable<int> LocalA() => Array.Empty<int>();
}

#nullable restore
#pragma warning restore IDE0005

""";

        // Pass the source code to our helper and snapshot test the output
        return Verify(source, new LoggerCodeGenerator());
    }

    public static int PublicMethod111()
    {
        return 0;
    }

    [Fact]
    private void serializer_json_string()
    {
        StructModel? m = new StructModel(1);

        var result1 = JsonSerializer.Serialize(m);

        var tuple = (1, 2, 3);
        var r2 = JsonSerializer.Serialize(tuple);
        var r3 = tuple.ToString();

        var d = (dynamic)1;

        var a = DynamicMethod(DynamicMethod).GetType();
    }

    private object DynamicMethod(object obj) => obj;

    private record struct StructModel(int Id) { }

    [Fact]
    public void probable_generated_method()
    {
        // literal
        var gv1 = new LiteralValue<System.Nullable<int>>(null);

        var r1 = gv1.ToString();

        var gv2 = new LiteralValue<System.Nullable<int>>(1);

        var r2 = gv2.ToString();

        // enumerable + literal
        var ev1 = new EnumerableValue();
        var l1 = new List<int> { 1, 2, 3 };
        foreach(var item in l1)
        {
            ev1.AddPropertyValue(new LiteralValue<int>(item));
        }

        var r3 = ev1.ToString();

        var ev2 = new EnumerableValue();
        var l2 = new List<Model> { new Model(1), new Model(2) };
        foreach(var item in l2)
        {
            ev2.AddPropertyValue(ToObjectValue(item));
        }

        var r4 = ev2.ToString();

        // dictionary + literal
        var dv1 = new DictionaryValue();
        var d1 = new Dictionary<int, Model>
        {
            {1 , new Model(1) },
            {2 , new Model(2) },
            {3 , new Model(3) },
        };
        foreach(var item in d1)
        {
            dv1.Add(item.Key.ToString(), ToObjectValue(item.Value));
        }

        var r5 = dv1.ToString();
    }

    private static ObjectValue ToObjectValue(Model model)
    {
        var v = new ObjectValue(typeof(Model).FullName!);

        v.AddProperty(new Property(nameof(Model.Id), new LiteralValue<int>(model.Id)));

        return v;
    }

    private class Model
    {
        public Model(int id) => Id = id;

        public int Id { get; }
    }
}

[Serializable]
public class User<T>
{
    public int Id { get; }

    public T Value { get; }
}

public interface IModel
{

}

public interface IModel<T> { }
