using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BepInEx.Analyzers
{
    public static class HarmonyUtil
    {
        // Check if the method or the class containing the method has Harmony related attributes
        public static bool IsMethodHarmonyRelated(ref SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, ISymbol symbol)
        {
            bool hasHarmonyAttributes = symbol.HasAttribute(TypeNames.HarmonyPatch);
            if (!hasHarmonyAttributes)
                if (method.Parent is ClassDeclarationSyntax classDeclaration)
                    if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken).HasAttribute(TypeNames.HarmonyPatch))
                        hasHarmonyAttributes = true;
            return hasHarmonyAttributes;
        }

        // Same as above but from a SuppressionAnalysisContext context
        public static bool IsMethodHarmonyRelated(ref SuppressionAnalysisContext context, SemanticModel semanticModel, MethodDeclarationSyntax method, ISymbol symbol)
        {
            bool hasHarmonyAttributes = symbol.HasAttribute(TypeNames.HarmonyPatch);
            if (!hasHarmonyAttributes)
                if (method.Parent is ClassDeclarationSyntax classDeclaration)
                    if (semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken).HasAttribute(TypeNames.HarmonyPatch))
                        hasHarmonyAttributes = true;
            return hasHarmonyAttributes;
        }
    }
}
