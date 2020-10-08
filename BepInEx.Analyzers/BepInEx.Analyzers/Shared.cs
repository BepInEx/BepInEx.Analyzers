using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace BepInEx.Analyzers
{
    public static class Shared
    {
        public static bool HasHarmonyAttributes(MemberDeclarationSyntax method)
        {
            foreach (var attributeList in method.AttributeLists)
                foreach (var attribute in attributeList.Attributes)
                    if (attribute.Name.ToString() == "HarmonyPatch")
                        return true;
            return false;
        }

        public static bool HasBepInPluginAttribute(ClassDeclarationSyntax classDeclaration)
        {
            foreach (var attributeList in classDeclaration.AttributeLists)
                foreach (var attribute in attributeList.Attributes)
                    if (attribute.Name.ToString() == "BepInPlugin")
                        return true;
            return false;
        }

        public static bool DerivesFromBaseUnityPlugin(ClassDeclarationSyntax classDeclaration)
        {
            if (classDeclaration.BaseList == null)
                return false;

            foreach (var node in classDeclaration.BaseList.ChildNodes())
                if (node is SimpleBaseTypeSyntax simpleBaseType)
                    if (simpleBaseType.ToString() == "BaseUnityPlugin")
                        return true;
            return false;
        }
    }
}
