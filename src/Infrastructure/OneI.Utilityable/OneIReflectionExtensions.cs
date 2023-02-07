namespace OneI;

using Cysharp.Text;

public static class OneIReflectionExtensions
{
    #region Type Display Name

    private const char DefaultNestedTypeDelimiter = '+';

    private static readonly Dictionary<Type, string> _builtInTypeNames = new()
    {
            { typeof(void), "void" },
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(object), "object" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" }
        };

    [return: NotNullIfNotNull(nameof(item))]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetTypeDisplayName(object? item, bool hasFullName = true)
    {
        return item == null ? null : GetTypeDisplayName(item.GetType(), hasFullName);
    }

    /// <summary>
    /// 获取指定类型<typeparamref name="T"/>的显示名称
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="hasFullName"></param>
    /// <returns>
    /// <para>For example, given <c><![CDATA[System.Collections.Generic.Dictionary<string, string>]]></c></para>
    /// <para>
    ///    when hasFullName is <see langword="true"/>, you will get <c><![CDATA[System.Collections.Generic.Dictionary<string, string>]]></c>
    /// </para>
    /// <para>
    ///    otherwise is <see langword="false"/>, you will get <c><![CDATA[Dictionary<string, string>]]></c>
    /// </para> 
    /// </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string? GetTypeDisplayName<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)] T>(bool hasFullName = true)
    {
        return GetTypeDisplayName(typeof(T), hasFullName);
    }

    /// <summary>
    /// 获取指定类型<paramref name="type"/>的显示名称
    /// </summary>
    /// <param name="type">The <see cref="Type"/>.</param>
    /// <param name="hasFullName"><c>true</c> to print a fully qualified name.</param>
    /// <param name="includeGenericParameterNames"><c>true</c> to include generic parameter names.</param>
    /// <param name="includeGenericParameters"><c>true</c> to include generic parameters.</param>
    /// <param name="nestedTypeDelimiter">Character to use as a delimiter in nested type names</param>
    /// <returns>
    /// <para>For example, given <c><![CDATA[System.Collections.Generic.Dictionary<string, string>]]></c></para>
    /// <para>
    ///    when hasFullName is <see langword="true"/>, you will get <c><![CDATA[System.Collections.Generic.Dictionary<string, string>]]></c>
    /// </para>
    /// <para>
    ///    otherwise is <see langword="false"/>, you will get <c><![CDATA[Dictionary<string, string>]]></c>
    /// </para> 
    /// </returns>
    public static string GetTypeDisplayName(
        Type type,
        bool hasFullName = true,
        bool includeGenericParameterNames = false,
        bool includeGenericParameters = true,
        char nestedTypeDelimiter = DefaultNestedTypeDelimiter)
    {
        var builder = ZString.CreateStringBuilder(true);

        try
        {
            ProcessType(ref builder, type, new DisplayNameOptions(hasFullName, includeGenericParameterNames, includeGenericParameters, nestedTypeDelimiter));

            return builder.ToString();
        }
        finally
        {
            builder.Dispose();
        }
    }

    private static void ProcessType(ref Utf16ValueStringBuilder builder, Type type, in DisplayNameOptions options)
    {
        if(type.IsGenericType)
        {
            var genericArguments = type.GetGenericArguments();

            ProcessGenericType(ref builder, type, genericArguments, genericArguments.Length, options);
        }
        else if(type.IsArray)
        {
            ProcessArrayType(ref builder, type, options);
        }
        else if(_builtInTypeNames.TryGetValue(type, out var builtInName))
        {
            builder.Append(builtInName);
        }
        else if(type.IsGenericParameter)
        {
            if(options.IncludeGenericParameterNames)
            {
                builder.Append(type.Name);
            }
        }
        else
        {
            var name = options.HasFullName ? type.FullName! : type.Name;

            builder.Append(name);
            if(options.NestedTypeDelimiter != DefaultNestedTypeDelimiter)
            {
                builder.Replace(DefaultNestedTypeDelimiter, options.NestedTypeDelimiter, builder.Length - name.Length, name.Length);
            }
        }
    }

    private static void ProcessArrayType(ref Utf16ValueStringBuilder builder, Type type, in DisplayNameOptions options)
    {
        var innerType = type;
        while(innerType.IsArray)
        {
            innerType = innerType.GetElementType()!;
        }

        ProcessType(ref builder!, innerType, options);

        while(type.IsArray)
        {
            builder.Append('[');
            builder.Append(',', type.GetArrayRank() - 1);
            builder.Append(']');
            type = type.GetElementType()!;
        }
    }

    private static void ProcessGenericType(ref Utf16ValueStringBuilder builder, Type type, Type[] genericArguments, int length, in DisplayNameOptions options)
    {
        var offset = 0;
        if(type.IsNested)
        {
            offset = type.DeclaringType!.GetGenericArguments().Length;
        }

        if(options.HasFullName)
        {
            if(type.IsNested)
            {
                ProcessGenericType(ref builder, type.DeclaringType!, genericArguments, offset, options);
                builder.Append(options.NestedTypeDelimiter);
            }
            else if(!string.IsNullOrEmpty(type.Namespace))
            {
                builder.Append(type.Namespace);
                builder.Append('.');
            }
        }

        var genericPartIndex = type.Name.IndexOf('`');
        if(genericPartIndex <= 0)
        {
            builder.Append(type.Name);
            return;
        }

        builder.Append(type.Name, 0, genericPartIndex);

        if(options.IncludeGenericParameters)
        {
            builder.Append('<');
            for(var i = offset; i < length; i++)
            {
                ProcessType(ref builder!, genericArguments[i], options);
                if(i + 1 == length)
                {
                    continue;
                }

                builder.Append(',');
                if(options.IncludeGenericParameterNames || !genericArguments[i + 1].IsGenericParameter)
                {
                    builder.Append(' ');
                }
            }

            builder.Append('>');
        }
    }

    private readonly struct DisplayNameOptions
    {
        public DisplayNameOptions(bool hasFullName, bool includeGenericParameterNames, bool includeGenericParameters, char nestedTypeDelimiter)
        {
            HasFullName = hasFullName;
            IncludeGenericParameters = includeGenericParameters;
            IncludeGenericParameterNames = includeGenericParameterNames;
            NestedTypeDelimiter = nestedTypeDelimiter;
        }

        public bool HasFullName { get; }

        public bool IncludeGenericParameters { get; }

        public bool IncludeGenericParameterNames { get; }

        public char NestedTypeDelimiter { get; }
    }

    #endregion Type Display Name End
}
