namespace OneI.Utilityable;

using System.Linq.Expressions;
using System.Reflection;
using DotNext.Reflection;
using Microsoft.Win32.SafeHandles;

public class InternalMethodReflectionBenchmark : BenchmarkItem
{
    const string MethodName = "FastAllocateString";
    const string FieldName = "_path";

    [Benchmark]
    [BenchmarkCategory("Call Method")]
    public MethodInfo UseReflection_Method()
    {
        var method = typeof(string).GetMethod(MethodName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);

        return method ?? throw new ArgumentNullException();
    }

    [Benchmark]
    [BenchmarkCategory("Call Method")]
    public MethodInfo UseDotNext_Method()
    {
        return Type<string>.Method<int>.RequireStatic<string>(MethodName, true);
    }

    [Benchmark]
    [BenchmarkCategory("Call Field")]
    public FieldInfo UseReflection_Field()
    {
        var field = typeof(SafeFileHandle).GetField(FieldName, BindingFlags.Instance | BindingFlags.NonPublic);

        return field ?? throw new ArgumentNullException();
    }

    [Benchmark]
    [BenchmarkCategory("Call Field")]
    public FieldInfo UseDotNext_Field()
    {
        var field = Type<SafeFileHandle>.Field<string>.Get(FieldName, true);

        return field ?? throw new ArgumentNullException();
    }

    [Benchmark]
    [BenchmarkCategory("Call Field")]
    public FieldInfo UseExpression_Field()
    {
        var type = typeof(SafeFileHandle);

        var parameter = Expression.Parameter(type, "handle");

        var field = Expression.Field(parameter, type, FieldName);

        var a = Expression.Lambda<Func<SafeFileHandle, string>>(field, parameter);

        var b = ExpressionPrinter.Print(a);

        return null!;
    }

    public override void Inlitialize()
    {
        UseReflection_Method();
        UseDotNext_Method();
        UseReflection_Field();
        UseDotNext_Field();
        UseExpression_Field();
    }
}
