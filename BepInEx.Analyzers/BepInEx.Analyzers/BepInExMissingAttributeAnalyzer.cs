using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BepInEx.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class BepInExMissingAttributeAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "BepInEx001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.BepInExMissingAttributeAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.BepInExMissingAttributeAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.BepInExMissingAttributeAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Class Declaration";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        }

        private static void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            if (HasBepInPluginAttribute(classDeclaration))
                return;

            if (!DerivesFromBaseUnityPlugin(classDeclaration))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier));
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
