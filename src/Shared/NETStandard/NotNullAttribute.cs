namespace System.Diagnostics.CodeAnalysis;

/// <summary>
/// Specifies that the method or property will ensure that the listed field and property members have not-null values.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, Inherited = false)]
internal sealed class NotNullAttribute : Attribute
{
}
