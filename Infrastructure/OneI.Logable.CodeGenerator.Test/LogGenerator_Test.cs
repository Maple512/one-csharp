namespace OneI.Logable;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using OneI.Logable.Templating;
using OneI.Logable.Templating.Properties.ValueTypes;
using OneT.CodeGenerator;
using VerifyXunit;
using Xunit;

[UsesVerify]
public class LogGenerator_Test : CodeGeneratorSnapshotTest
{
    [Fact]
    public Task generator_simple()
    {
        // The source code to test
        var source = """
#nullable enable
namespace Tests;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using OneI.Logable;

public class UserService
{
    public delegate string Handlers();

    public event Handlers HandlerEvent;

    public enum UserType { a, b, c, d }

    private void Index<T0, T1>(T0 t0, T1 t1)
        where T0 : new()
    {
        var p1 = (object)1;
        var p2 = UserType.a;

        HandlerEvent += () => string.Empty;
        HandlerEvent += () => string.Empty;
        var p3 = HandlerEvent;

        var p4 = 1m;
        var p5 = true;
        var p6 = 'c';
        sbyte p7 = 1;
        var p8 = new object[] { 1, 2, 3, 4, 5 };

        var p9 = new BitArray(new[] { true, false });

        var p10 = new List<int> { 1, 2, 3, 4 };

        var p11 = (1, 2, 3);

        Log.Debug("message text", p1, new object(), p2, UserType.b, p3, p4, 3, p5, false, p6, '0', p7, (sbyte)10, p8, new object[] { "1", 2, 'c' }, p9, new BitArray(new[] { true, false, }), p10, new List<byte> { 1, 2, 3 }, p11);
    }
}

#nullable restore
""";

        var directory = typeof(object).Assembly.Location;

        // Pass the source code to our helper and snapshot test the output
        return Verify(source, new LoggerCodeGenerator());
    }

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
