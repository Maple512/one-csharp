namespace OneI.Logable.Internal;

using OneI.Logable.Definitions;

internal partial class CodePrinter
{
    private const string arg_name = "args";

    private static void PrintType(IndentedStringBuilder builder, TypeDef type)
    {
        builder.AppendLine("[MethodImpl(MethodImplOptions.AggressiveInlining)]");
        builder.AppendLine($"static object? {type.WrapperMethod}({type.ToDisplayString()} {arg_name})");

        builder.AppendLine("{");

        using(var _ = builder.Indent())
        {
            builder.Append($"return ");
            switch(type.Kind)
            {
                case TypeDefKind.Array:
                case TypeDefKind.Dictionary:
                case TypeDefKind.EnumerableT:
                case TypeDefKind.Tuple:
                case TypeDefKind.ValueTuple:
                case TypeDefKind.Object:
                    builder.Append($"new {type.FormatterName}({arg_name})");
                    break;
                case TypeDefKind.Nullable:
                    builder.Append($"{arg_name}.HasValue ? {type.TypeArguments[0].WrapperMethod}({arg_name}.Value) : null");
                    break;
                default:
                    builder.Append(arg_name);
                    break;
            }

            builder.AppendLine($";");
        }

        builder.AppendLine("}");
    }
}
