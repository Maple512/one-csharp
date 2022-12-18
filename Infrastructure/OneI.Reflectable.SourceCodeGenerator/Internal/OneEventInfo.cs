namespace OneI.Reflectable.Internal;

using System;
using System.Reflection;

public class OneEventInfo : EventInfo
{
    public override EventAttributes Attributes { get; }
    public override Type DeclaringType { get; }
    public override string Name { get; }
    public override Type ReflectedType { get; }

    public override MethodInfo GetAddMethod(bool nonPublic)
    {
        throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(bool inherit)
    {
        throw new NotImplementedException();
    }

    public override object[] GetCustomAttributes(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }

    public override MethodInfo GetRaiseMethod(bool nonPublic)
    {
        throw new NotImplementedException();
    }

    public override MethodInfo GetRemoveMethod(bool nonPublic)
    {
        throw new NotImplementedException();
    }

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }
}
