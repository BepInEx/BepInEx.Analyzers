using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BepInEx.Analyzers
{
    public static class Shared
    {
        public static bool HasHarmonyAttributes(MethodDeclarationSyntax method)
        {
            foreach (var attributeList in method.AttributeLists)
                foreach (var attribute in attributeList.Attributes)
                    if (attribute.Name.ToString() == "HarmonyPatch")
                        return true;
            return false;
        }
    }
}
