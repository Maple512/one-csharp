namespace OneI.Reflectable;

using System.Collections.Generic;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public class ModuleWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var md = ModuleDefinition;

        // Write a log entry with a specific MessageImportance
        WriteMessage("OneI.Reflectable.Fody: Hello World.", MessageImportance.High);

        var ns = GetNamespace();
        var type = new TypeDefinition(ns, "Hello", TypeAttributes.Public, TypeSystem.ObjectReference);

        AddConstructor(type);

        AddHelloWorld(type);

        ModuleDefinition.Types.Add(type);
        WriteInfo("Added type 'Hello' with method 'World'.");
    }

    /// <summary>
    /// 解析类型时所需的程序集
    /// </summary>
    /// <returns></returns>
    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
    }

    private string GetNamespace()
    {
        var namespaceFromConfig = GetNamespaceFromConfig();
        var namespaceFromAttribute = GetNamespaceFromAttribute();
        if(namespaceFromConfig != null && namespaceFromAttribute != null)
        {
            throw new WeavingException("Configuring namespace from both Config and Attribute is not supported.");
        }

        if(namespaceFromAttribute != null)
        {
            return namespaceFromAttribute;
        }

        return namespaceFromConfig;
    }

    private string GetNamespaceFromConfig()
    {
        var attribute = Config?.Attribute("Namespace");
        if(attribute == null)
        {
            return null;
        }

        var value = attribute.Value;
        ValidateNamespace(value);
        return value;
    }

    private string GetNamespaceFromAttribute()
    {
        var attributes = ModuleDefinition.Assembly.CustomAttributes;
        var namespaceAttribute = attributes
            .SingleOrDefault(x => x.AttributeType.FullName == "OneI.Reflectable.NamespaceAttribute");
        if(namespaceAttribute == null)
        {
            return null;
        }

        attributes.Remove(namespaceAttribute);
        var value = (string)namespaceAttribute.ConstructorArguments.First().Value;
        ValidateNamespace(value);
        return value;
    }

    private static void ValidateNamespace(string value)
    {
        if(value is null || string.IsNullOrWhiteSpace(value))
        {
            throw new WeavingException("Invalid namespace");
        }
    }

    private void AddConstructor(TypeDefinition newType)
    {
        var attributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        var method = new MethodDefinition(".ctor", attributes, TypeSystem.VoidReference);
        var objectConstructor = ModuleDefinition.ImportReference(TypeSystem.ObjectDefinition.GetConstructors().First());
        var processor = method.Body.GetILProcessor();
        processor.Emit(OpCodes.Ldarg_0);
        processor.Emit(OpCodes.Call, objectConstructor);
        processor.Emit(OpCodes.Ret);
        newType.Methods.Add(method);
    }

    private void AddHelloWorld(TypeDefinition newType)
    {
        var method = new MethodDefinition("World", MethodAttributes.Public, TypeSystem.StringReference);
        var processor = method.Body.GetILProcessor();
        processor.Emit(OpCodes.Ldstr, "Hello World");
        processor.Emit(OpCodes.Ret);
        newType.Methods.Add(method);
    }

    public override bool ShouldCleanReference => true;
}
