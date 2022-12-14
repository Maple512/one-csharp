namespace OneE.EnumCodeGenerator;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public class EnumToStringGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddSource(SourceCodeHelper.AttributeFileName, SourceText.From(SourceCodeHelper.Attribute, Encoding.UTF8));
        });

        var hasEnumSyntaxes = context.SyntaxProvider.CreateSyntaxProvider(
            static (s, _) => IsSyntaxTargetForGeneration(s),
            static (ctx, _) => GetSemanticTargetForGeneration(ctx));

        var compilation = context.CompilationProvider
            .Combine(hasEnumSyntaxes.Collect());

        context.RegisterSourceOutput(compilation,
            static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is EnumDeclarationSyntax m && m.AttributeLists.Count > 0;
    }

    private static EnumDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext cts)
    {
        var enumSyntax = (EnumDeclarationSyntax)cts.Node;

        foreach(var attributeListSyntax in enumSyntax.AttributeLists)
        {
            foreach(var attributeSyntax in attributeListSyntax.Attributes)
            {
                if(cts.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingType = attributeSymbol.ContainingType;

                var fullname = attributeContainingType.ToDisplayString();

                if(fullname == SourceCodeHelper.AttributeFullName)
                {
                    return enumSyntax;
                }
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<EnumDeclarationSyntax?> enums, SourceProductionContext context)
    {
        if(enums.IsDefaultOrEmpty)
        {
            // nothing to do yet
            return;
        }

        // I'm not sure if this is actually necessary, but `[LoggerMessage]` does it, so seems like a good idea!
        var distinctEnums = enums!.Distinct();

        // Convert each EnumDeclarationSyntax to an EnumToGenerate
        var enumsToGenerate = GetTypesToGenerate(compilation, distinctEnums, context.CancellationToken);

        // If there were errors in the EnumDeclarationSyntax, we won't create an
        // EnumToGenerate for it, so make sure we have something to generate
        if(enumsToGenerate.Count > 0)
        {
            // generate the source code and add it to the output
            var result = SourceCodeHelper.GenerateExtensionClass(enumsToGenerate);
            context.AddSource("EnumExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }

    private static List<EnumToGenerate> GetTypesToGenerate(Compilation compilation, IEnumerable<EnumDeclarationSyntax?> enums, CancellationToken ct)
    {
        // Create a list to hold our output
        var enumsToGenerate = new List<EnumToGenerate>();

        // Get the semantic representation of our marker attribute
        var enumAttribute = compilation.GetTypeByMetadataName(SourceCodeHelper.AttributeFullName);

        if(enumAttribute == null)
        {
            // If this is null, the compilation couldn't find the marker attribute type
            // which suggests there's something very wrong! Bail out..
            return enumsToGenerate;
        }

        foreach(var enumDeclarationSyntax in enums)
        {
            // stop if we're asked to
            ct.ThrowIfCancellationRequested();

            // Get the semantic representation of the enum syntax
            var semanticModel = compilation.GetSemanticModel(enumDeclarationSyntax!.SyntaxTree);
            if(semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol enumSymbol)
            {
                // something went wrong, bail out
                continue;
            }

            // Get the full type name of the enum e.g. Colour,
            // or OuterClass<T>.Colour if it was nested in a generic type (for example)
            var enumName = enumSymbol.ToString()!;

            // Get all the members in the enum
            var enumMembers = enumSymbol.GetMembers();
            var members = new List<string>(enumMembers.Length);

            // Get all the fields from the enum, and add their name to the list
            foreach(var member in enumMembers)
            {
                if(member is IFieldSymbol field && field.ConstantValue is not null)
                {
                    members.Add(member.Name);
                }
            }

            // Create an EnumToGenerate for use in the generation phase
            enumsToGenerate.Add(new EnumToGenerate(enumName, members));
        }

        return enumsToGenerate;
    }
}

public readonly struct EnumToGenerate
{
    public readonly string Name;
    public readonly List<string> Values;

    public EnumToGenerate(string name, List<string> values)
    {
        Name = name;
        Values = values;
    }
}
