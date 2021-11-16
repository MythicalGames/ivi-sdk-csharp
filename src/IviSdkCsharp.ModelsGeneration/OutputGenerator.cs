using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;

namespace IviSdkCsharp.ModelsGeneration
{
    internal static class OutputGenerator
    {
        private const string Indent = "\t";
        private const string DoubleIndent = "\t\t";
        private const string TripleIndent = "\t\t\t";
        private const string MythicalNamespace = "Mythical.Game.IviSdkCSharp.Model";

        internal static string GenerateClass(INamedTypeSymbol targetType, HashSet<string> namespaces, string modelName)
        {
            
            var allModelProps = ModelsGenerator.GetAllProperties(targetType).ToArray();

            var props = new StringBuilder();
            Dictionary<string, PropertyType> modelProperties = new(allModelProps.Length);
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
                var name = modelProp.Name;
                modelProperties[name] = propertyType;
                props.Append(DoubleIndent).Append("public ").Append(propertyType.Value).Append(" ").Append(name)
                    .Append(" { get; set; }")
                    .AppendLine();
            }

            string constructors = GenerateConsructors(modelName, modelProperties);

            return $@"{string.Join(Environment.NewLine, namespaces)}

namespace {MythicalNamespace}
{{    
{Indent}public partial class {modelName}
{Indent}{{
{constructors}
{props}
{Indent}}}
}}
";
        }

        private static string GenerateConsructors(string modelName, Dictionary<string, PropertyType> modelProperties)
        {
            StringBuilder result = new(DoubleIndent);
            result.Append("public ").Append(modelName).Append("()").AppendLine();
            result.Append(DoubleIndent).Append("{");
            foreach (var prop in modelProperties.Where(x => x.Value.ForPropertyType.IsReferenceType))
            {
                if (prop.Value.ForPropertyType.SpecialType == SpecialType.System_String)
                {
                    AddPropertyInitialize(prop.Key, "string.Empty");
                }
                else
                {
                    AddPropertyInitialize(prop.Key, $"new {prop.Value.Value}()");
                }
            }
            result.AppendLine().Append(DoubleIndent).Append("}");
            return result.ToString();

            void AddPropertyInitialize(string name, string initValue)
            {
                result.AppendLine();
                result.Append(TripleIndent).Append(name);
                result.Append(" = ");
                result.Append(initValue).Append(";");
            }
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

        private static PropertyType GetPropertyType(bool isGoogle, bool isGrpcGenerated, IPropertySymbol modelProp)
        {
            if (isGrpcGenerated)
            {
                return new PropertyType(ModelsGenerator.ModelPrefix + modelProp.Type.Name, modelProp);
            }

            if (isGoogle)
            {
                var typeName = modelProp.Type.Name;
                if (typeName == "Struct")
                    return new PropertyType("Dictionary<string, object>", modelProp)
                    {
                    };
                if (typeName == "RepeatedField" &&
                    ((INamedTypeSymbol) modelProp.Type).TypeArguments is
                    {Length: 1} genericTypes && genericTypes[0] is INamedTypeSymbol collectionType )
                {
                    var ns = collectionType.ContainingNamespace;
                    return new PropertyType(
                        (ns?.IsGlobalNamespace ?? true) || collectionType.SpecialType != SpecialType.None
                            ? $"List<{SimplifyTypeName(collectionType)}>"
                            : $"List<{ns.ToDisplayString()}.{collectionType.Name}>",
                        modelProp)
                    {
                    };
                }
            }
            
            return new PropertyType(SimplifyTypeName(modelProp.Type), modelProp);
        }

        private static string SimplifyTypeName(ITypeSymbol type) => type.SpecialType switch
        {
            SpecialType.System_String => "string",
            SpecialType.System_Int32 => "int",
            SpecialType.System_Int64 => "long",
            SpecialType.System_Boolean => "bool",
            _ => type.Name
        };

        private class PropertyType
        {
            public PropertyType(string value, IPropertySymbol forProperty)
            {
                Value = value;
                ForProperty = forProperty;
            }
            public readonly string Value;
            public readonly IPropertySymbol ForProperty;
            public ITypeSymbol ForPropertyType => ForProperty.Type;
        }
    }
}