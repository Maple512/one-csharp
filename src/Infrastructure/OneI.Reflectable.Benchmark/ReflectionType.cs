namespace OneI.Reflectable;

using System.Reflection;
using System.Reflection.Emit;
using InlineIL;

using static InlineIL.IL;
using static InlineIL.IL.Emit;

public class ReflectionType
{
    private readonly Model m = new Model();
    private readonly Func<Type, string> _getFullName;

    public ReflectionType()
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        var type = typeof(Type);
        var method = type.GetMethod("get_FullName", flags)!;

        var dm = new DynamicMethod(
            "DYnamicMethod1",
            typeof(string),
            new[] { typeof(Type) },
            true);

        var il = dm.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Callvirt, method);
        il.Emit(OpCodes.Ret);

        _getFullName = dm.CreateDelegate<Func<Type, string>>();
    }

    [Benchmark]
    public string Typeof_FullName()
    {
        return typeof(Model).FullName!;
    }

    [Benchmark]
    public string GetType_FullName()
    {
        return m.GetType().FullName!;
    }

    [Benchmark]
    public string InfoOf_FullName()
    {
        var type = Info.OfType("OneI.Reflectable.Benchmark", "OneI.Reflectable.Model");

        return type.FullName!;
    }

    [Benchmark]
    public string InlineIL_FullName()
    {
        Ldtoken<Model>();

        Call(new MethodRef(typeof(Type), "GetTypeFromHandle"));

        Callvirt(new MethodRef(typeof(Type), "get_FullName"));

        return Return<string>();
    }


    [Benchmark]
    public string IL_FullName()
    {
        return _getFullName.Invoke(typeof(Model));
    }

    //[Benchmark]
    //public FieldInfo Typeof_Field()
    //{
    //    return typeof(Model).GetField("id", BindingFlags.Instance | BindingFlags.Public)!;
    //}

    //[Benchmark]
    //public FieldInfo GetType_Field()
    //{
    //    return m.GetType().GetField("id", BindingFlags.Instance | BindingFlags.Public)!;
    //}

    //[Benchmark]
    //public PropertyInfo Typeof_Property()
    //{
    //    return typeof(Model).GetProperty("Name", BindingFlags.Instance | BindingFlags.Public)!;
    //}

    //[Benchmark]
    //public PropertyInfo GetType_Property()
    //{
    //    return m.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public)!;
    //}
}
