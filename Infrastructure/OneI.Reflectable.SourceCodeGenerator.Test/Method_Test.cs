namespace OneI.Reflectable;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using OneI.Diagnostics;

public unsafe class Method_Test
{
    private const string InvokePrefix = "OneI_Reflectable_";

    [Fact]
    public void create_delegate_from_dynamie_to_call_private_method()
    {
        var model = new Model1();
        var fieldName = "<Id>k__BackingField";

        var setter3 = CreaSetter<Model1, int>(fieldName);

        var field = typeof(Model1).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;

        var setter2 = CreateSetter<Model1, int>(field);

        DebugWatch.Start(category: "model.Id = 11");

        model.Id = 512;

        DebugWatch.Mark("SetValue(model, value)");

        var property = typeof(Model1).GetProperty(nameof(Model1.Id))!;
        property.SetValue(model, 513);

        DebugWatch.Mark("ILGenerator");

        setter2.Invoke(model, 514);

        DebugWatch.Mark("Expression");

        setter3.Invoke(model, 515);

        DebugWatch.EndAndReport();
    }

    private static Action<TTYpe, TParameter> CreateSetter<TTYpe, TParameter>(FieldInfo field)
    {
        var dm = new DynamicMethod(
            $"{InvokePrefix}{nameof(TTYpe)}",
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

    private static Action<TType, TParameter> CreaSetter<TType, TParameter>(string fieldName)
    {
        var p1 = Expression.Parameter(typeof(TType), "m");
        var p2 = Expression.Parameter(typeof(TParameter), "v");

        var field = Expression.Field(p1, fieldName);

        var assign = Expression.Assign(field, p2);

        return Expression.Lambda<Action<TType, TParameter>>(assign, new[] { p1, p2 }).Compile();
    }

    [Fact]
    public void lambda_expression_access_field()
    {
        var p1 = Expression.Parameter(typeof(Model2), "m");

        var field = Expression.Field(p1, "id");

        var value = Expression.Constant(12);

        var assign = Expression.Assign(field, value);

        var setter = Expression.Lambda<Action<Model2>>(assign, new[] { p1 });

        var result = ExpressionPrinter.Print(setter);

        var m = new Model2();

        setter.Compile().Invoke(m);
    }

    private class Model1
    {
        public int Id { get; set; }
    }

    private class Model2
    {
        private int id;
    }
}
