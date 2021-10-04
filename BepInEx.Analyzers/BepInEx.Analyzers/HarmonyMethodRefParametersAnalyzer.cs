using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace BepInEx.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HarmonyMethodRefParametersAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Harmony003";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.HarmonyMethodRefParametersAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.HarmonyMethodRefParametersAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.HarmonyMethodRefParametersAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Method Declaration";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(method, context.CancellationToken);

            if (!HarmonyUtil.IsMethodHarmonyRelated(ref context, method, symbol))
                return;

            if (method.ParameterList?.Parameters.Count <= 0)
                return;

            var assignments = context.Node.DescendantNodes().OfType<AssignmentExpressionSyntax>();

            foreach (var assignment in assignments)
            {
                var varName = assignment.Left.ToString();

                foreach (var parameter in method.ParameterList.Parameters)
                {
                    var parameterSplit = parameter.ToString().Split(' ');
                    if (parameterSplit.Any(s => s == varName) && !parameterSplit.Any(s => s == "ref"))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(Rule, assignment.GetLocation(), assignment.ToString()));
                    }
                }
            }

        }
    }
}
