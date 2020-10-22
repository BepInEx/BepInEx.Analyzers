using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Threading;

namespace BepInEx.Analyzers
{
    public static class Extensions
    {
        public static T GetRawSymbol<T>(this SemanticModel semanticModel, SyntaxNode syntax) where T : class, ISymbol
        {
            var symbol = semanticModel.GetSymbolInfo(syntax).Symbol;

            if(symbol == null)
                symbol = semanticModel.GetDeclaredSymbol(syntax);

            if(symbol == null)
                symbol = semanticModel.GetPreprocessingSymbolInfo(syntax).Symbol;

            if(symbol == null)
                return symbol as T;
            else
                return symbol.OriginalDefinition as T;
        }

        public static bool InheritsFrom(this ClassDeclarationSyntax classDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken, string typeName)
        {
            var symbol = semanticModel.GetDeclaredSymbol(classDeclaration, cancellationToken);

            while(true)
            {
                if(symbol.ToString() == typeName)
                {
                    return true;
                }

                if(symbol.BaseType != null)
                {
                    symbol = symbol.BaseType;
                    continue;
                }

                break;
            }

            return false;
        }

        public static bool HasAttribute(this MemberDeclarationSyntax memberDeclaration, SemanticModel semanticModel, CancellationToken cancellationToken, string typeName)
        {
            foreach(var attributeList in memberDeclaration.AttributeLists)
                foreach(var attribute in attributeList.Attributes)
                {
                    var symbol = semanticModel.GetSymbolInfo(attribute, cancellationToken).Symbol;
                    if(symbol != null && symbol.ContainingType.ToString() == typeName)
                        return true;
                }

            return false;
        }

        public static bool IsStatic(this MethodDeclarationSyntax methodDeclaration)
        {
            return methodDeclaration.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword);
        }
    }
}
