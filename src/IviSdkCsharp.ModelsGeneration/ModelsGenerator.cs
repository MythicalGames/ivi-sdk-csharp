using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace IviSdkCsharp.ModelsGeneration;

[Generator]
public class ModelsGenerator : ISourceGenerator
{
    public void Execute(GeneratorExecutionContext context)
    {
        var finder = (GrpcTypesFinder?)context.SyntaxReceiver;


        if (finder is { TargetTypes: { Count: > 0 } iviTypes })
        {
            var model = context.Compilation.GetSemanticModel(finder.Root!.SyntaxTree);
            var types = iviTypes.Select(x => (INamedTypeSymbol)model.GetTypeInfo(x.Type)!.Type!).ToArray();
            var ordered = OrderTypes(context, types);
            foreach (var targetType in ordered)
            {
                GenerateModel(context, targetType);
            }
        }
        else
        {
            Log(context, "no config");
        }
    }

    public IEnumerable<INamedTypeSymbol> OrderTypes(GeneratorExecutionContext context, IEnumerable<INamedTypeSymbol> types)
    {
        var result = new Stack<INamedTypeSymbol>();
        var work = new Queue<INamedTypeSymbol>(types);
        HashSet<string> seen = new();

        while (work.Count > 0)
        {
            var current = work.Dequeue();
            if (!seen.Contains(current.Name))
            {
                seen.Add(current.Name);
                result.Push(current);

                if (current.TypeKind == TypeKind.Enum) continue;

                var properties = GetAllProperties(current);
                foreach (var child in properties)
                {
                    var childType = child.Type;
                    if (IsGrpcGeneratedNamespace(childType.ContainingNamespace))
                    {
                        work.Enqueue((INamedTypeSymbol)childType);
                    }
                }
            }
        }
        return result.ToArray();
    }

    internal static IPropertySymbol[] GetAllProperties(INamedTypeSymbol type) =>
        type.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x =>
                !PropTypesToSkip.Contains(x.Type.Name)
                && !x.GetAttributes().Any(attr => attr.AttributeClass?.Name == nameof(ObsoleteAttribute)))
            .ToArray();

    internal static bool IsGrpcGeneratedNamespace(INamespaceSymbol? ns)
        => ns?.ToDisplayString().StartsWith("Ivi.Proto") is true;

    internal const string ModelPrefix = "Ivi";

    private void GenerateModel(GeneratorExecutionContext context,
        INamedTypeSymbol targetType)
    {
        var modelName = GetModelName(targetType);
        HashSet<string> namespaces = new()
        {
            "using System;",
            "using System.Collections.Generic;"
        };

        string source = targetType.TypeKind == TypeKind.Enum
            ? OutputGenerator.GenerateEnum(targetType, namespaces, modelName)
            : OutputGenerator.GenerateClass(targetType, namespaces, modelName);
        context.AddSource(modelName + ".cs", source);
        Log(context, "Generating code for " + targetType.Name);
    }

    public void Initialize(GeneratorInitializationContext context)
        => context.RegisterForSyntaxNotifications(() => new GrpcTypesFinder());

    private string GetModelName(INamedTypeSymbol type)
        => ModelPrefix + (type.Name.StartsWith("ivi", StringComparison.OrdinalIgnoreCase) ? type.Name.Substring(3) : type.Name);

    private static void Log(GeneratorExecutionContext context, string message, string title = "test")
    {
        context.ReportDiagnostic(
            Diagnostic.Create(
                new DiagnosticDescriptor(title, nameof(ModelsGenerator), "{0}", "model generation", DiagnosticSeverity.Info, true),
                Location.None,
                message
            ));
    }

    internal static readonly HashSet<string> PropTypesToSkip = new()
    {
        "MessageParser",
        "MessageDescriptor"
    };
}