namespace OneI.Reflectable;

using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnostics.Windows.Configs;

[RankColumn, MemoryDiagnoser, NativeMemoryProfiler, ThreadingDiagnoser, DisassemblyDiagnoser]
[InliningDiagnoser(true, true)]
public class SetValueForPrivateField
{
    private const string _field_name = "Id";

    private static Action<Model1, int> _il;
    private static Action<Model1, int> _lambda;
    private static Action<Model1, int> _proprety;

    public SetValueForPrivateField()
    {
        var field = typeof(Model1).GetField(_field_name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;

        _il = CreateSetter<Model1, int>(field);

        _lambda = CreateSetter<Model1, int>(_field_name);

        _proprety = (m, i) => field.SetValue(m, i);
    }

    // 3
    [Benchmark(Baseline = true)]
    public void UseProperty()
    {
        var m = new Model1();
        _proprety(m, 512);
        Debugger.Break();
    }

    // 2
    [Benchmark]
    public void UseIL()
    {
        var m = new Model1();
        _il(m, 512);
        Debugger.Break();
    }

    // 1
    [Benchmark]
    public void UseLambda()
    {
        var m = new Model1();
        _lambda(m, 512);
        Debugger.Break();
    }

    private static Action<TTYpe, TParameter> CreateSetter<TTYpe, TParameter>(FieldInfo field)
    {
        var dm = new DynamicMethod(
            $"OneI_Reflectable_{nameof(TTYpe)}",
            typeof(void),
           new[] { typeof(TTYpe), typeof(TParameter) },
           false);

        var il = dm.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);// 将索引处的参数推送到计算堆栈上
        il.Emit(OpCodes.Ldarg_1);// 将索引处的参数推送到计算堆栈上

        il.Emit(OpCodes.Stfld, field);// 用新值替换在对象引用或指针的字段中存储的值

        il.Emit(OpCodes.Ret); // 从当前方法返回，并将返回值（如果存在）从调用方的计算堆栈推送到被调用方的计算堆栈上。

        return dm.CreateDelegate<Action<TTYpe, TParameter>>();
    }

    private static Action<TType, TParameter> CreateSetter<TType, TParameter>(string fieldName)
    {
        var p1 = Expression.Parameter(typeof(TType), "m");
        var p2 = Expression.Parameter(typeof(TParameter), "v");

        var field = Expression.Field(p1, fieldName);

        var assign = Expression.Assign(field, p2);

        return Expression.Lambda<Action<TType, TParameter>>(assign, new[] { p1, p2 }).Compile();
    }

    private class Model1
    {
        private int Id;
    }
}
