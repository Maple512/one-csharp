namespace OneI.Utilityable;

using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.Win32.SafeHandles;

public class InternalMethodReflectionBenchmark : IValidator
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
        return DotNext.Reflection.Type<string>.Method<int>.RequireStatic<string>(MethodName, true);
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
        var field = DotNext.Reflection.Type<SafeFileHandle>.Field<string>.Get(FieldName, true);

        return field ?? throw new ArgumentNullException();
    }

    [Benchmark]
    [BenchmarkCategory("Call Field")]
    public FieldInfo UseExpression_Field()
    {
        var type = typeof(SafeFileHandle);

        var parameter = Expression.Parameter(type, "handle");

        var field = Expression.Field(parameter, type, FieldName);

        var a = Expression.Lambda<Func<SafeFileHandle, string>>(field, new[] { parameter });

        var b = ExpressionPrinter.Print(a);

        return null;
    }

    void IValidator.Validate()
    {
        UseReflection_Method();
        UseDotNext_Method();
        UseReflection_Field();
        UseDotNext_Field();
        UseExpression_Field();
    }
}
