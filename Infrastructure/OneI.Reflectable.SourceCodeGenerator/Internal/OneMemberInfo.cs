namespace OneI.Reflectable.Internal;

using System;
using System.Reflection;

public class OneMemberInfo : MemberInfo
{
    public override Type DeclaringType { get; }
    public override MemberTypes MemberType { get; }
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

    public override bool IsDefined(Type attributeType, bool inherit)
    {
        throw new NotImplementedException();
    }
}
