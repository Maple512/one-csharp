namespace OneI.Reflectable.Internal;

using System;
using System.Globalization;
using System.Reflection;

public class OneConstructorInfo : ConstructorInfo
{
    public override MethodAttributes Attributes { get; }
    public override RuntimeMethodHandle MethodHandle { get; }
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

    public override MethodImplAttributes GetMethodImplementationFlags()
    {
        throw new NotImplementedException();
    }

    public override ParameterInfo[] GetParameters()
    {
        throw new NotImplementedException();
    }

    public override object Invoke(BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }
}
