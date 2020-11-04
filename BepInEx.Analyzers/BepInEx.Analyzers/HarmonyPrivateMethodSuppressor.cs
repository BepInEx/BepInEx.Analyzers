using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

namespace BepInEx.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HarmonyPrivateMethodSuppressor : DiagnosticSuppressor
    {
        public const string DiagnosticId = "HarmonySuppressor001";
        public const string SuppressedDiagnosticId = "IDE0051";

        private static readonly LocalizableString Justification = new LocalizableResourceString(nameof(Resources.HarmonyPrivateMethodSuppressorJustification), Resources.ResourceManager, typeof(Resources));

        internal static readonly SuppressionDescriptor Rule = new SuppressionDescriptor(DiagnosticId, SuppressedDiagnosticId, Justification);

        public override void ReportSuppressions(SuppressionAnalysisContext context)
        {
            foreach (var diagnostic in context.ReportedDiagnostics)
                AnalyzeDiagnostic(diagnostic, context);
        }

        public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions => ImmutableArray.Create(Rule);

        private void AnalyzeDiagnostic(Diagnostic diagnostic, SuppressionAnalysisContext context)
        {
            Location location = diagnostic.Location;
            SyntaxTree sourceTree = location.SourceTree;
            SemanticModel semanticModel = context.GetSemanticModel(sourceTree);
            SyntaxNode root = sourceTree.GetRoot(context.CancellationToken);
            SyntaxNode node = root.FindNode(location.SourceSpan);

            if (!(node is MethodDeclarationSyntax method))
                return;

            var symbol = semanticModel.GetDeclaredSymbol(method, context.CancellationToken);

            //Check if the method or the class containing the method has Harmony related attributes
            bool hasHarmonyAttributes = symbol.HasAttribute(TypeNames.HarmonyPatch);
            if (!hasHarmonyAttributes)
                if (method.Parent is ClassDeclarationSyntax classDeclaration)
                    if (symbol.HasAttribute(TypeNames.HarmonyPatch))
                        hasHarmonyAttributes = true;
            if (!hasHarmonyAttributes)
                return;

            foreach(var descriptor in SupportedSuppressions.Where(d => d.SuppressedDiagnosticId == diagnostic.Id))
                context.ReportSuppression(Suppression.Create(descriptor, diagnostic));
        }
    }
}
