namespace OneI.Logable.Properties.Policies;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// 反结构策略
/// 将对象从结构表示为复杂的日志属性
/// </summary>
public interface IDestructuringPolicy
{
    bool TryDestructure(
        object value,
        IPropertyValueFactory propertyValueFactory,
        [NotNullWhen(true)] out PropertyValue? propertyValue);
}
