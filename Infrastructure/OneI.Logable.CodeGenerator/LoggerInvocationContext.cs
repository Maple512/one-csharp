//namespace OneI.Logable;

//using System.Collections.Generic;
//using OneI.Logable.Definitions;

//public class LoggerInvocationContext
//{
//    public LoggerInvocationContext(int capacity)
//    {
//        Parameties = new(capacity);
//    }

//    public List<ParameterDefinition> Parameties { get; }

//    public Dictionary<string, string?> GenericConstraints
//        => Parameties.Where(x => x.Type.IsMethodConstraints)
//        .ToDictionary((k => k.Type.Name), v => v.Type.GenericConstraints);

//    public LoggerInvocationContext AddParameter(string typeName)
//    {
//        if(string.IsNullOrWhiteSpace(typeName) == false)
//        {
//            var order = Parameties.Count;
//            Parameties.Add(new ParameterDefinition(order, typeName));
//        }

//        return this;
//    }

//    public LoggerInvocationContext AddParameters(params TypeDefinition[] types)
//    {
//        if(types.Length > 0)
//        {
//            var order = Parameties.Count;
//            foreach(var item in types)
//            {
//                Parameties.Add(new(order++, item));
//            }
//        }

//        return this;
//    }

//    public LoggerInvocationContext AddGenericConstraint(string name, string? constraint)
//    {
//        if(string.IsNullOrWhiteSpace(name) == false)
//        {
//            if(GenericConstraints.ContainsKey(name))
//            {
//                GenericConstraints[name] = constraint;
//            }
//            else
//            {
//                GenericConstraints.Add(name, constraint);
//            }
//        }

//        return this;
//    }

//    public string Build(IndentedStringBuilder content)
//    {
//        content.Append($"public static void Write");

//        if(GenericConstraints.Count > 0)
//        {
//            content.Append($"<{string.Join(", ", GenericConstraints.Keys)}>");
//        }

//        content.AppendLine("(");
//        using(var _ = content.Indent())
//        {
//            foreach(var item in Parameties)
//            {
//                content.AppendLine($"{item},");
//            }

//            content.AppendLine("[CallerMemberName] string? memberName = null,");
//            content.AppendLine("[CallerFilePath] string? filePath = null,");
//            content.Append("[CallerLineNumber] int? lineNumber = null");
//            content.AppendLine(")");

//            // 泛型约束
//            foreach(var x in GenericConstraints
//                .Where(x => string.IsNullOrWhiteSpace(x.Value) == false))
//            {
//                content.AppendLine($"where {x.Key}: {x.Value}");
//            }
//        }

//        content.AppendLine("{");

//        // body


//        content.AppendLine("}");

//        return content.ToString();
//    }
//}
