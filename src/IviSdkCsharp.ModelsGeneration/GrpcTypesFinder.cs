using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace IviSdkCsharp.ModelsGeneration
{
    public class GrpcTypesFinder: ISyntaxReceiver
    {
        public List<PropertyDeclarationSyntax> TargetTypes = new();
        public ClassDeclarationSyntax? Root;

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax {Identifier: {ValueText: "IviModelsGenerationConfig"}} config)
            {

                Root = config;
                TargetTypes.AddRange(config.Members.OfType<PropertyDeclarationSyntax>());
            }
        }
    }
}