using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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

        public static bool InheritsFrom(this SyntaxNodeAnalysisContext context, string typeName)
        {
            var symbol = context.SemanticModel.GetRawSymbol<INamedTypeSymbol>(context.Node);

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

        public static bool HasAttribute(this SyntaxNodeAnalysisContext context, string typeName)
        {
            foreach(var attributeList in ((ClassDeclarationSyntax)context.Node).AttributeLists)
                foreach(var attribute in attributeList.Attributes)
                {
                    var symbol = context.SemanticModel.GetRawSymbol<ISymbol>(attribute);
                    if(symbol.ContainingType.ToString() == typeName)
                        return true;
                }

            return false;
        }
    }
}
