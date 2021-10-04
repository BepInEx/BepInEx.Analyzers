using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BepInEx.Analyzers
{
    public static class HarmonyUtil
    {
        // Check if the method is Harmony patch related :
        // It is when its static and the HarmonyPatch attribute is present
        // either on the method or the containing class
        // Todo : When the attribute is on the class,
        // check if the method name is any of the following :
        // Prefix, Postfix, Transpiler, Finalizer etc
        public static bool IsMethodHarmonyPatchRelated(ref SyntaxNodeAnalysisContext context, MethodDeclarationSyntax method, ISymbol symbol)
        {
            if (!method.IsStatic())
            {
                return false;
            }

            bool hasHarmonyAttributes = symbol.HasAttribute(TypeNames.HarmonyPatch);
            if (!hasHarmonyAttributes)
                if (method.Parent is ClassDeclarationSyntax classDeclaration)
                    if (context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken).HasAttribute(TypeNames.HarmonyPatch))
                        hasHarmonyAttributes = true;
            return hasHarmonyAttributes;
        }

        // Same as above but from a SuppressionAnalysisContext context
        public static bool IsMethodHarmonyPatchRelated(ref SuppressionAnalysisContext context, SemanticModel semanticModel, MethodDeclarationSyntax method, ISymbol symbol)
        {
            if (!method.IsStatic())
            {
                return false;
            }

            bool hasHarmonyAttributes = symbol.HasAttribute(TypeNames.HarmonyPatch);
            if (!hasHarmonyAttributes)
                if (method.Parent is ClassDeclarationSyntax classDeclaration)
                    if (semanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken).HasAttribute(TypeNames.HarmonyPatch))
                        hasHarmonyAttributes = true;
            return hasHarmonyAttributes;
        }
    }
}
