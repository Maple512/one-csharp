namespace System.Diagnostics;

/// <summary>
/// Types and Methods attributed with StackTraceHidden will be omitted from the stack trace text shown in StackTrace.ToString()
/// and Exception.StackTrace
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Struct, Inherited = false)]
internal sealed class StackTraceHiddenAttribute : Attribute
{
}
