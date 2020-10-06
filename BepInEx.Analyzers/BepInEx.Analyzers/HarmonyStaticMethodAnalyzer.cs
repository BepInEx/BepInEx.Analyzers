using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using static BepInEx.Analyzers.Shared;

namespace BepInEx.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HarmonyStaticMethodAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Harmony001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.HarmonyStaticMethodAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.HarmonyStaticMethodAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.HarmonyStaticMethodAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Method Declaration";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var method = (MethodDeclarationSyntax)context.Node;
            if (method.Modifiers.Any(x => x.Kind() == SyntaxKind.StaticKeyword))
                return;

            if (!HasHarmonyAttributes(method))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, method.Identifier.GetLocation(), method.Identifier));
        }
    }
}
