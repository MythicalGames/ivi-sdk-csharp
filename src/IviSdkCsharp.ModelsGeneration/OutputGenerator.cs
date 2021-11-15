using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace IviSdkCsharp.ModelsGeneration
{
    internal static class OutputGenerator
    {
        private const string Indent = "\t";
        private const string MythicalNamespace = "Mythical.Game.IviSdkCSharp.Model";

        internal static string GenerateClass(INamedTypeSymbol targetType, HashSet<string> namespaces, string modelName)
        {
            
            var allModelProps = targetType.GetMembers()
                .OfType<IPropertySymbol>().Where(x => !ModelsGenerator.PropTypesToSkip.Contains(x.Type.Name)).ToArray();


            var props = new StringBuilder();
            foreach (var modelProp in allModelProps)
            {
                var ns = modelProp.Type.ContainingNamespace;
                var isGoogle = ns!.ToDisplayString().StartsWith("Google.Protobuf"); // Google.Protobuf.WellKnownTypes.Struct, Google.Protobuf.Collections.RepeatableField<T>
                var isGrpcGenerated = ModelsGenerator.IsGrpcGeneratedNamespace(ns);
                if (!ns.IsGlobalNamespace && !isGoogle && !isGrpcGenerated )
                {
                    namespaces.Add($"using {ns.ToDisplayString()};");
                }

                var propertyType = GetPropertyType(isGoogle, isGrpcGenerated, modelProp);
                props.Append(Indent).Append(Indent).Append("public ").Append(propertyType).Append(" ").Append(modelProp.Name)
                    .Append(" { get; set; }")
                    .AppendLine();
            }

            return $@"{string.Join(Environment.NewLine, namespaces)}

namespace {MythicalNamespace}
{{    
{Indent}public partial class {modelName}
{Indent}{{
{props}
{Indent}}}
}}
";
        }

        internal static string GenerateEnum(INamedTypeSymbol targetType, HashSet<string> namespaces, string modelName)
        {
            var enumSyntax = targetType.DeclaringSyntaxReferences[0].GetSyntax();
            var enumDefinition = Regex.Replace(enumSyntax.ToString(), @"\[.+]\s+", "", RegexOptions.Multiline)
                .Replace(targetType.Name, modelName);
            return $@"{string.Join(Environment.NewLine, namespaces)}

namespace {MythicalNamespace}
{{    
{enumDefinition}
}}
";
        }

        private static string GetPropertyType(bool isGoogle, bool isGrpcGenerated, IPropertySymbol modelProp)
        {
            if (isGrpcGenerated)
            {
                return ModelsGenerator.ModelPrefix + modelProp.Type.Name;
            }

            if (isGoogle)
            {
                var typeName = modelProp.Type.Name;
                if (typeName == "Struct")
                    return "Dictionary<string, object>";
                if (typeName == "RepeatedField" &&
                    ((Microsoft.CodeAnalysis.INamedTypeSymbol) modelProp.Type).TypeArguments is
                    {Length: 1} genericTypes && genericTypes[0] is INamedTypeSymbol collectionType )
                {
                    var ns = collectionType.ContainingNamespace;
                    return (ns?.IsGlobalNamespace ?? true) ? $"List<{collectionType.Name}>" : $"List<{ns.ToDisplayString()}.{collectionType.Name}>";
                }
            }
            
            return modelProp.Type.Name;
        }
    }
}