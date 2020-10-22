using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BepInEx.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HarmonyPropertyPatchAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Harmony002";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.HarmonyPropertyPatchAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.HarmonyPropertyPatchAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.HarmonyPropertyPatchAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Method Declaration";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.Attribute);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var attribute = (AttributeSyntax)context.Node;

            var symbol = context.SemanticModel.GetSymbolInfo(attribute, context.CancellationToken).Symbol;
            if (symbol != null && symbol.ContainingType.ToString() == "HarmonyLib.HarmonyPatch")
            {
                foreach (var argument in attribute.ArgumentList.Arguments)
                {
                    var expression = argument.Expression;

                    if (expression is LiteralExpressionSyntax literal)
                    {
                        var str = literal.ToString();
                        int strStart = str.IndexOf("\"") + 1;
                        int strEnd = str.LastIndexOf("\"");
                        str = str.Substring(strStart, strEnd - strStart);

                        if (str.StartsWith("get_") || str.StartsWith("set_"))
                        {
                            context.ReportDiagnostic(Diagnostic.Create(Rule, context.Node.GetLocation()));
                            return;
                        }
                    }
                }
            }
        }
    }
}
