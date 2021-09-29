using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace BepInEx.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class AccessPublicizedMemberAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Publicizer001";

        private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AccessPublicizedMemberAnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AccessPublicizedMemberAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        private static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AccessPublicizedMemberAnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        private const string Category = "Member Access";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, true, Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        private const string PublicizedAttributeName = "System.Runtime.CompilerServices.PublicizedAttribute";

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeSimpleMemberAccess, SyntaxKind.SimpleMemberAccessExpression);
        }

        private void AnalyzeSimpleMemberAccess(SyntaxNodeAnalysisContext context)
        {
            var memberAccess = (MemberAccessExpressionSyntax)context.Node;
            var symbol = context.SemanticModel.GetSymbolInfo(memberAccess.Name, context.CancellationToken).Symbol;

            if (symbol == null)
                return;

            if (IsContextInheritingFromPublicizedType(symbol.ContainingType, context))
                return;

            if (IsPublicized(symbol))
            {
                context.ReportDiagnostic(Diagnostic.Create(Rule, memberAccess.Name.GetLocation(), memberAccess.Name.Identifier));
            }
        }

        private static bool IsContextInheritingFromPublicizedType(INamedTypeSymbol containingType, SyntaxNodeAnalysisContext context)
        {
            var contextBaseType = context.ContainingSymbol.ContainingType?.BaseType;
            while (contextBaseType != null)
            {
                if (SymbolEqualityComparer.Default.Equals(containingType, contextBaseType))
                {
                    return true;
                }

                contextBaseType = contextBaseType.BaseType;
            }

            return false;
        }

        private static bool IsPublicized(ISymbol symbol) =>
            symbol.HasAttribute(PublicizedAttributeName);
    }
}
