namespace OneI.Reflectionable;

using System;
using System.Globalization;
using System.Reflection;

public class Field : FieldInfo
{
    public Field()
    {
    }

    public override FieldAttributes Attributes { get; }
    public override RuntimeFieldHandle FieldHandle { get; }
    public override Type FieldType { get; }
    public override Type DeclaringType { get; }
    public override string Name { get; }
    public override Type ReflectedType { get; }

    public override object[] GetCustomAttributes(bool inherit)
    {
        throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }

    public override object GetValue(object obj)
    {
        throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }

    public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
