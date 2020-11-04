using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

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

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(classDeclaration, context.CancellationToken);

            if(symbol.HasAttribute(TypeNames.BepInPlugin))
                return;

            if(!symbol.InheritsFrom(TypeNames.BaseUnityPlugin))
                return;

            if(context.Compilation.Assembly.GlobalNamespace.GetAllTypes().Any(x => x.InheritsFrom(symbol.ToString()) && x.HasAttribute(TypeNames.BepInPlugin)))
                return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation(), classDeclaration.Identifier));
        }
    }
}
